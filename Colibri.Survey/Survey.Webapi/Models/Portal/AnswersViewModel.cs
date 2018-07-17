﻿using Survey.ApplicationLayer.Dtos.Models.Answers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Survey.Webapi.Models.Portal
{
    public class AnswersViewModel
    {
        public Guid SurveyId { get; set; }
        public List<object> AnswerList { get; set; }

    }
}
