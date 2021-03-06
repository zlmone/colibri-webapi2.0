﻿using AutoMapper;
using Survey.ApplicationLayer.Dtos.Entities;
using Survey.ApplicationLayer.Dtos.Models;
using Survey.ApplicationLayer.Dtos.Models.IdentityServer;
using Survey.ApplicationLayer.Dtos.Models.Questions;
using Survey.ApplicationLayer.Dtos.Search;
using Survey.DomainModelLayer.Entities;
using Survey.DomainModelLayer.Models.IdentityServer;
using Survey.DomainModelLayer.Models.Search;

namespace Survey.ApplicationLayer.Configurations.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            EntityToDto();
            DtoToEntity();
            ViewModelToEntity();
        }


        private void DtoToEntity()

        {
            // models dto
            CreateMap<GroupModel, GroupDto>();
            CreateMap<SearchQueryModel, SearchQueryDto>();
            CreateMap<MemberModel, MemberDto>();
            CreateMap<IdentityUserModel, IdentityUserDto>();
            // entities dto
            CreateMap<SurveySectionDto, SurveySections>();
            CreateMap<PagesDto, Pages>();
            CreateMap<QuestionsDto, Questions>();
            CreateMap<InputTypesDto, InputTypes>();
            CreateMap<OptionGroupsDto, OptionGroups>();
            CreateMap<OptionChoisesDto, OptionChoises>();
            CreateMap<UsersDto, Users>();
        }
        private void EntityToDto()
        {
            // to viewModel
            CreateMap<SurveySections, SurveyExtendViewModel>();
            // models
            CreateMap<SearchQueryDto, SearchQueryModel>();
            CreateMap<MemberDto, MemberModel>();
            CreateMap<SearchQueryPageDto, SearchQueryPageModel>();
            CreateMap<FilterStatementDto, FilterStatementModel>();
            CreateMap<OrderStatementDto, OrderStatementModel>();
            CreateMap(typeof(SearchResultDto<>), typeof(SearchResultModel<>));
            CreateMap<SearchResultPageDto, SearchResultPageModel>();
            CreateMap<IdentityUserDto, IdentityUserModel>();
            CreateMap<IdentityUserDto, IdentityUserModel>();
            CreateMap<SurveySections, SurveyModel>();
            // entities
            CreateMap<SurveySections, SurveySectionDto>();
            CreateMap<Pages, PagesDto>();
            CreateMap<Questions, QuestionsDto>();
            CreateMap<InputTypes, InputTypesDto>();
            CreateMap<OptionGroups, OptionGroupsDto>();
            CreateMap<OptionChoises, OptionChoisesDto>();
            CreateMap<Users, UsersDto>();
            //CreateMap<PersonalClaimDto, PersonalClaim>();
        }

        private void ViewModelToEntity()
        {
            // models
            CreateMap<BaseQuestionModel, Questions>()
                .ForMember(dest => dest.OrderNo, opt => opt.MapFrom(src => src.Order));

        }
    }
}
