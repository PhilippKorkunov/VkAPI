
using BusinessLayer.Implementations;
using DataLayer.Entities;

namespace BusinessLayer
{
    public class DataManager
    {
        public EFRepository<User> UsersRepository { get; set; }
        public EFRepository<UserGroup> UserGroupsRepository { get; set; }
        public EFRepository<UserState> UserStatesRepository { get; set; }
        public EFRepository<UserWithFullInfo> FullInfoUserRepository { get; set; }

        public DataManager(EFRepository<User> userFRepository, 
            EFRepository<UserGroup> userGroupRepository, EFRepository<UserState> userStateFRepository,
            EFRepository<UserWithFullInfo> userWithFullInfo) 
        { 
            UsersRepository = userFRepository;
            UserGroupsRepository = userGroupRepository;
            UserStatesRepository = userStateFRepository;
            FullInfoUserRepository = userWithFullInfo;
        }

    }
}
