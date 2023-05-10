using DataLayer.Entities;
using DataLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VkAppAPI.ModelViews;


namespace VkAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VkController : ControllerBase
    {

        private readonly BusinessLayer.DataManager dataManager;
        public VkController(BusinessLayer.DataManager dataManager) 
        {
            this.dataManager = dataManager;
        }

        [HttpGet("GetFullInfoAllUser")]
        public async Task<IActionResult> GetAllUsersAsync() // Получить всех пользователей
        {
            await using var fullInfoUserRep = dataManager.FullInfoUserRepository;
            return(Ok(from user in await fullInfoUserRep.GetAsync()
               select new FullInfoUserModelView(user)));
        } 

        [HttpGet("GetFullInfoOneUser")]
        public async Task<IActionResult> GetOneUserAsync(int id)//Получить пользователя по id
        {
            await using var fullInfoUserRep = dataManager.FullInfoUserRepository;
            var user = (await fullInfoUserRep.GetAsync(user => user.Id == id)).FirstOrDefault();

            if (user is null) { return BadRequest("user hasn't found"); }

            return Ok(new FullInfoUserModelView(user));
        }

        [HttpGet("GetOnePageFullInfoUsers")]
        public async Task<IActionResult> GetOnePageUsersAsync(int pageNumber, int pageSize) //Get с ПАГИНАЦИЕЙ, возвращающий страницу с №pageNumber
        {

            await using var fullInfoUserRep = dataManager.FullInfoUserRepository;

            var users = (await fullInfoUserRep.GetAsync())
                                               .Skip((pageNumber - 1) * pageSize)
                                               .Take(pageSize)
                                               .ToList();

            if (pageNumber > users.Count)
            {
                return BadRequest("Required pages are more then users in DB");
            }

            return Ok(from user in users
                      select new FullInfoUserModelView(user));
        }

        [HttpGet("GetAllPagesFullInfoUsers")]
        public async Task<IActionResult> GetOnePageUsersAsync(int pageSize) //Пагинацией с возвращением всех пользователей через список вложенных списков
        {
            await using var fullInfoUserRep = dataManager.FullInfoUserRepository;
            List<List<UserWithFullInfo>> pagedUsers = new();

            var users = (await fullInfoUserRep.GetAsync()).ToList();
     
            int totalPages = (int)Math.Ceiling(users.Count / (double)pageSize);

            for (int i = 0; i < totalPages; i++)
            {
                pagedUsers.Add(new List<UserWithFullInfo>());
                pagedUsers[i] = users.Skip((i) * pageSize).Take(pageSize).ToList();
            }
            return Ok(pagedUsers);
        }

        [HttpGet("GetWithBaseAuth")]
        public async Task<IActionResult> GetOnePageUsersAsync()
        {
            return Ok();
        }


        [HttpPost("AddUser")]
        public async Task<IActionResult> Post(JsonDocument jsonDocument) //Добавление нового пользователя
        {
            var jsonDict = new Dictionary<string, string>();
            try
            {
                jsonDict = jsonDocument.ToJsonDict();
            }
            catch (Exception ex)
            {
                return BadRequest("Неверный формат входных данныз\n" + ex.Message);
            }

            string[] requiredKeys = new string[] { "login", "password", "userGroupCode" };
            string message = String.Empty;
            if (!JsonConvertor.ValidateJsonKeys(jsonDict, requiredKeys, out message)) // проверка ключей в json
            {
                return BadRequest(message);
            }

            var allowedValues = new string[] { "User", "Admin" };
            if (!JsonConvertor.ValidateJsonValues(jsonDict["userGroupCode"], allowedValues, out message)) // проверка значений в json
            {
                return BadRequest(message);
            }

            var userGroup = (DataLayer.Enums.UserGroup)Enum.Parse(typeof(DataLayer.Enums.UserGroup), jsonDict["userGroupCode"]);

            await using var userRepository = dataManager.UsersRepository;
            await using var userStateRepository = dataManager.UserStatesRepository;
            await using var userGroupRepository = dataManager.UserGroupsRepository;


            var currentUserState = (await userStateRepository.GetAsync(state => state.Code == DataLayer.Enums.UserState.Active)).FirstOrDefault(); //новый пользоваетль получает статуч active
            var currentUserGroup = (await userGroupRepository.GetAsync(group => group.Code == userGroup)).FirstOrDefault();

            if (currentUserState is not null && currentUserGroup is not null)
            {
                if (userGroup == DataLayer.Enums.UserGroup.Admin && (await userRepository.GetAsync(x => x.UserGroupId == currentUserGroup.Id)).Any())
                {
                    return BadRequest("Admin already exits. It's allowed to have only 1 admin"); // Не позволяет иметь более 1 админа
                }

                User user = new()
                {
                    Login = jsonDict["login"],
                    Password = jsonDict["password"],
                    CreatedDate = DateTime.UtcNow,
                    UserGroupId = currentUserGroup.Id,
                    UserStateId = currentUserState.Id  
                };

                return Ok(await userRepository.InsertAsync(user)); 
            }
            else
            {
                return BadRequest($"Either user state or user group is null:\n" +
                    $"User state is null : {currentUserState is null}\nUser group is null: {currentUserGroup is null}");
            }
        }



        [HttpPut("BlockUserById")]
        public async Task<IActionResult> BlockUserById(int id)
        {
            await using var userReposotory = dataManager.UsersRepository;
            await using var userStatesRepository = dataManager.UserStatesRepository;
            await using var userGroupRepository = dataManager.UserGroupsRepository;

            var currentUserGroup = (await userGroupRepository.GetAsync(group => group.Code == DataLayer.Enums.UserGroup.Admin)).FirstOrDefault();
            var user = (await dataManager.UsersRepository.GetAsync(user => user.Id == id)).FirstOrDefault();

            if (currentUserGroup is null) { return BadRequest("Requestd group is null"); }
            if (user is null) { return BadRequest("Requestd user is null"); }

            if (user.UserGroupId == currentUserGroup.Id) { return BadRequest("It's impossible to block admin"); }

            var state = (await dataManager.UserStatesRepository.GetAsync(state => state.Code == DataLayer.Enums.UserState.Blocked)).FirstOrDefault();
            if (state is null) { return BadRequest("User State is null"); }

            var stateId = state.Id;

            if (user.UserStateId == stateId) { return Ok("User has been already blocked"); }

            user.UserStateId = stateId;
            return Ok(await dataManager.UsersRepository.UpdateAsync(user));
        }

        private async Task FillGroupAndState() // Добавление статусов и ролей в соответвующие таблицы
        {
            await using var userStatesRepository = dataManager.UserStatesRepository;
            await using var userGroupRepository = dataManager.UserGroupsRepository;

            await userGroupRepository.InsertAsync(new DataLayer.Entities.UserGroup()
            {
                Code = DataLayer.Enums.UserGroup.User,
                Description = "Обычный пользователь"
            });

            await userGroupRepository.InsertAsync(new DataLayer.Entities.UserGroup()
            {
                Code = DataLayer.Enums.UserGroup.Admin,
                Description = "Админ"
            });

            await userStatesRepository.InsertAsync(new DataLayer.Entities.UserState()
            {
                Code = DataLayer.Enums.UserState.Active,
                Description = "Активный пользователь"
            });

            await userStatesRepository.InsertAsync(new DataLayer.Entities.UserState()
            {
                Code = DataLayer.Enums.UserState.Blocked,
                Description = "Заблокированный пользователь"
            });
        }
    }
}
