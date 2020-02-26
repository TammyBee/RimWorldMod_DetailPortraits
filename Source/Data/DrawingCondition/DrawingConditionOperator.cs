﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetailPortraits.Data.DrawingCondition {
    public enum DrawingConditionOperator {
        Equal,
        GT,
        GTE,
        LT,
        LTE,
        In,
        Contains,
        Shared,
        IsEmpty
    }
}
