﻿using AutoMapper;
using Survey.ApplicationLayer.Dtos.Models.IdentityServer;
using Survey.ApplicationLayer.Dtos.Search;
using Survey.ApplicationLayer.Services.Interfaces;
using Survey.DomainModelLayer.Models.IdentityServer;
using Survey.DomainModelLayer.Models.Search;
using Survey.InfrastructureLayer.IdentityServerServices.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Survey.ApplicationLayer.Services
{
    public class MemberService : IMemberService
    {
        private readonly IGroupMemberRequestService _groupMemberRequestService;
        protected readonly IMapper _mapper;

        public MemberService(
            IGroupMemberRequestService groupMemberRequestService,
            IMapper mapper
        ) {
            _groupMemberRequestService = groupMemberRequestService;
            _mapper = mapper;
        }

        public async Task<SearchResultDto<MemberDto>> GetMembersAsync(string groupId, SearchQueryDto searchEntryDto)
        {
            var searchEntry = _mapper.Map<SearchQueryDto, SearchQueryModel>(searchEntryDto);
            var result = await _groupMemberRequestService.GetMembers(groupId, searchEntry);
            var list = _mapper.Map<SearchResultModel<MemberModel>, SearchResultDto<MemberDto>>(result);
            return list;
        }

        public async Task AddMembersAsync(string groupId, IEnumerable<string> emailList)
        {
            await _groupMemberRequestService.AddMembers(groupId, emailList);
            return ;
        }

        public async Task UnsubscribeMemberAsync(string groupId, string memberId)
        {
            await _groupMemberRequestService.UnsubscribeMember(groupId, memberId);
            return;
        }
    }
}
