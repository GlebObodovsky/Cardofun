namespace Cardofun.Core.NameConstants
{
    public class PolicyConstants
    {
        public const string AdminRoleRequired = nameof(AdminRoleRequired);
        public const string ModeratorRoleRequired = nameof(ModeratorRoleRequired);
        /// <summary>
        /// One of the parameters that passed to the action should named as userId
        /// </summary>
        /// <returns></returns>
        public const string UserMatchRequired = nameof(UserMatchRequired);
    }
}