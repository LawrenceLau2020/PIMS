using AutoMapper;
using Entity = Pims.Dal.Entities;
using KModel = Pims.Keycloak.Models;

namespace Pims.Api.Areas.Keycloak.Profiles.Resolvers
{
    /// <summary>
    /// GroupToRoleResolver class, provides a way for automapper to convert a gropu to a role.
    /// </summary>
    public class GroupToRoleResolver : IValueResolver<KModel.GroupModel, Entity.Role, Entity.Role>
    {
        public Entity.Role Resolve(KModel.GroupModel source, Entity.Role destination, Entity.Role destMember, ResolutionContext context)
        {
            return source == null ? null : new Entity.Role(source.Id, source.Name);
        }
    }
}