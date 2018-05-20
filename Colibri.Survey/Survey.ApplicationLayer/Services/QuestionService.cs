﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using storagecore.Abstractions.Uow;
using Survey.ApplicationLayer.Dtos.Entities;
using Survey.ApplicationLayer.Dtos.Models;
using Survey.ApplicationLayer.Dtos.Models.Questions;
using Survey.ApplicationLayer.Services.Interfaces;
using Survey.Common.Enums;
using Survey.DomainModelLayer.Entities;

namespace Survey.ApplicationLayer.Services
{



    public class QuestionService : IQuestionService
    {
        private readonly IOptionChoiceService _optionChoiceService;
        private readonly IOptionGroupService _optionGroupService;
        private readonly IInputTypeService _inputTypeService;
        QuestionTypes type;

        protected readonly IUowProvider UowProvider;
        protected readonly IMapper Mapper;


        private Guid pageId;
        private BaseQuestionModel baseQuestionModel;

        private Dictionary<Type, Action> switchQuestionType;
        private IEnumerable<InputTypesDto> inputTypeList;

        public QuestionService(
            IUowProvider uowProvider,
            IMapper mapper,
            IInputTypeService inputTypeService,
            IOptionGroupService optionGroupService,
            IOptionChoiceService optionChoiceService
        )
        {
            this.UowProvider = uowProvider;
            this.Mapper = mapper;
            this._inputTypeService = inputTypeService;
            this._optionGroupService = optionGroupService;
            this._optionChoiceService = optionChoiceService;

            inputTypeList = _inputTypeService.GetAll();



            switchQuestionType = new Dictionary<Type, Action> {
                //{ typeof(TextQuestionModel), () =>
                //    {
                //        SaveTextQuestion( (TextQuestionModel)baseQuestionModel );
                //    }
                //},
                //{ typeof(TextAreaQuestionModel), () =>
                //    {
                //        SaveTextAreaQuestion( (TextAreaQuestionModel)baseQuestionModel );
                //    }
                //},
                //{ typeof(RadioQuestionModel), () =>
                //    {
                //        SaveRadioQuestion( (RadioQuestionModel)baseQuestionModel );
                //    }
                //},
                //{ typeof(CheckBoxQuesstionModel), () =>
                //    {
                //        SaveCheckBoxQuestion( (CheckBoxQuesstionModel)baseQuestionModel );
                //    }
                //},
                //{ typeof(DropdownQuestionModel), () =>
                //    {
                //        SaveDropdownQuestion( (DropdownQuestionModel)baseQuestionModel );
                //    }
                //},
                {  typeof(GridRadioQuestionModel), async () =>
                    {
                        await SaveGridRadioQuestion( (GridRadioQuestionModel)baseQuestionModel );
                    }
                },
            };
        }




        public List<BaseQuestionModel> GetTypedQuestionList(PageModel page)
        {
            List<BaseQuestionModel> baseQuestionList = new List<BaseQuestionModel>();

            if (page.Questions != null)
            {
                foreach (var baseQuestion in page.Questions)
                {
                    BaseQuestionModel question = GetQuestionByType(baseQuestion.ToString());
                    baseQuestionList.Add(question);
                }
            }
            return baseQuestionList;
        }

        private async Task<Guid> SaveQuestion(BaseQuestionModel data, bool IsAllowMultipleOptionAnswers, Guid? optionGroupId, Guid typeid, Guid? ParentId)
        {
            QuestionsDto questionDto = new QuestionsDto()
            {
                Name = data.Text,
                ParentId = ParentId != null ? ParentId : null,
                Description = data.Description,
                Required = data.Required,
                OrderNo = data.Order,
                AdditionalAnswer = data.IsAdditionalAnswer,
                AllowMultipleOptionAnswers = false,
                InputTypesId = typeid,
                PageId = pageId,
                OptionGroupId = optionGroupId
            };

            using (var uow = UowProvider.CreateUnitOfWork())
            {
                try
                {
                    Questions questionEntity = Mapper.Map<QuestionsDto, Questions>(questionDto);
                    var repositoryQuestion = uow.GetRepository<Questions, Guid>();
                    await repositoryQuestion.AddAsync(questionEntity);
                    await uow.SaveChangesAsync();

                    return questionEntity.Id;
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    throw;
                }
            }
        }

        //private async void SaveTextQuestion(TextQuestionModel data)
        //{
        //    Guid optionGroupId = await _optionGroupService.AddAsync();
        //    InputTypesDto type = inputTypeList.Where(item => item.Name == data.ControlType).FirstOrDefault();
        //    await SaveQuestion(data, false, optionGroupId, type.Id, null);
        //    await _optionChoiceService.AddAsync(optionGroupId, null);
        //}

        //private async void SaveTextAreaQuestion(TextAreaQuestionModel data)
        //{
        //    Guid optionGroupId = await _optionGroupService.AddAsync();
        //    InputTypesDto type = inputTypeList.Where(item => item.Name == data.ControlType).FirstOrDefault();
        //    await SaveQuestion(data, false, optionGroupId, type.Id, null);
        //    await _optionChoiceService.AddAsync(optionGroupId, null);
        //}

        //private async void SaveRadioQuestion(RadioQuestionModel data)
        //{
        //    Guid optionGroupId = _optionGroupService.AddAsync().Result;
        //    InputTypesDto type = inputTypeList.Where(item => item.Name == data.ControlType).FirstOrDefault();
        //    await SaveQuestion(data, false, optionGroupId, type.Id, null);
        //    if (data.Options.Count() > 0)
        //    {
        //        await _optionChoiceService.AddRangeAsync(optionGroupId, data.Options);
        //    }
        //}

        //private async void SaveCheckBoxQuestion(CheckBoxQuesstionModel data)
        //{
        //    Guid optionGroupId = _optionGroupService.AddAsync().Result;
        //    InputTypesDto type = inputTypeList.Where(item => item.Name == data.ControlType).FirstOrDefault();
        //    await SaveQuestion(data, true, optionGroupId, type.Id, null);
        //    if (data.Options.Count() > 0)
        //    {
        //        await _optionChoiceService.AddRangeAsync(optionGroupId, data.Options);
        //    }
        //}

        //private async void SaveDropdownQuestion(DropdownQuestionModel data)
        //{
        //    Guid optionGroupId = _optionGroupService.AddAsync().Result;
        //    InputTypesDto type = inputTypeList.Where(item => item.Name == data.ControlType).FirstOrDefault();
        //    await SaveQuestion(data, false, optionGroupId, type.Id, null);
        //    if (data.Options.Count() > 0)
        //    {
        //        await _optionChoiceService.AddRangeAsync(optionGroupId, data.Options);
        //    }
        //}

        private async Task<Guid> SaveGridRadioQuestion(GridRadioQuestionModel data)
        {
            
            InputTypesDto type = inputTypeList.Where(item => item.Name == data.ControlType).FirstOrDefault();
            var parentQuestionId = await SaveQuestion(data, false, null, type.Id, null);
            if (data.Grid.Rows.Count() > 0)
            {
                foreach (var item in data.Grid.Rows)
                {
                    Guid optionGroupId = _optionGroupService.AddAsync().Result;
                    await SaveQuestion(data, false, optionGroupId, type.Id, parentQuestionId);

                    if (data.Grid.Cols.Count() > 0)
                    {
                        await _optionChoiceService.AddRangeAsync(optionGroupId, data.Grid.Cols);
                    }
                    //await _optionChoiceService.AddRangeAsync(optionGroupId, data.Options);
                }
                
            }
            return type.Id;
        }






        public BaseQuestionModel GetQuestionByType(string baseQuestion)
        {
            BaseQuestionModel baseQuestionM = JsonConvert.DeserializeObject<BaseQuestionModel>(baseQuestion);
            if (Enum.TryParse(baseQuestionM.ControlType.ToString(), out type))
            {
                switch (type)
                {
                    case QuestionTypes.Textbox:
                        {
                            TextQuestionModel question = JsonConvert.DeserializeObject<TextQuestionModel>(baseQuestion);
                            baseQuestionM = question as BaseQuestionModel;
                            break;
                        }
                    case QuestionTypes.Textarea:
                        {
                            TextAreaQuestionModel question = JsonConvert.DeserializeObject<TextAreaQuestionModel>(baseQuestion);
                            baseQuestionM = question as BaseQuestionModel;
                            break;
                        }
                    case QuestionTypes.Radio:
                        {
                            RadioQuestionModel question = JsonConvert.DeserializeObject<RadioQuestionModel>(baseQuestion);
                            baseQuestionM = question as BaseQuestionModel;
                            break;
                        }
                    case QuestionTypes.Checkbox:
                        {
                            CheckBoxQuesstionModel question = JsonConvert.DeserializeObject<CheckBoxQuesstionModel>(baseQuestion);
                            baseQuestionM = question as BaseQuestionModel;
                            break;
                        }
                    case QuestionTypes.Dropdown:
                        {
                            DropdownQuestionModel question = JsonConvert.DeserializeObject<DropdownQuestionModel>(baseQuestion);
                            baseQuestionM = question as BaseQuestionModel;
                            break;
                        }
                    case QuestionTypes.GridRadio:
                        {
                            GridRadioQuestionModel question = JsonConvert.DeserializeObject<GridRadioQuestionModel>(baseQuestion);
                            baseQuestionM = question as BaseQuestionModel;

                            break;
                        }
                }
            }
            return baseQuestionM;
        }

        public void SaveQuestionByType(BaseQuestionModel baseQuestion, Guid id)
        {
            baseQuestionModel = baseQuestion;
            pageId = id;
            //var type = baseQuestion.GetType();
            switchQuestionType[baseQuestion.GetType()]();

        }
    }
}
