﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Survey.ApplicationLayer.Dtos.Models.Report
{
    public class AnswerModel
    {
        public string AnswerText { get; set; }
        public string OptionChoise { get; set; }
        public bool IsAdditional { get; set; }
        public bool AnswerBoolean { get; set; }

    }
}
