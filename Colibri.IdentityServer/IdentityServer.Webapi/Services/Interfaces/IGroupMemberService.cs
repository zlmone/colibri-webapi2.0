﻿using IdentityServer.Webapi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Webapi.Services.Interfaces
{
    public interface IGroupMemberService
    {
        Task<bool> AddMembersToGroupAsync(Guid groupId, List<string> emailList);
        Task<IEnumerable<ApplicationUser>> GetMembersForGroupAsync(Guid groupId);
        Task DeleteMember(string userId, Guid groupId);
    }
}