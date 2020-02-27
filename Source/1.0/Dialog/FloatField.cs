using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DetailPortraits.Dialog {
    public class FloatField {
        private string buffer = "";
        private float value;

        public float Value {
            get {
                return value;
            }
        }

        public FloatField(float value) {
            this.value = value;
            this.buffer = value.ToString();
        }

        public bool Update(Rect rect) {
            buffer = Widgets.TextField(rect, buffer);
            if (IsFullyTypedFloat(buffer)) {
                float x;
                if (float.TryParse(buffer, out x)) {
                    this.value = x;
                    return true;
                }
            }
            return false;
        }

        private static bool IsFullyTypedFloat(string str) {
            if (str == string.Empty) {
                return false;
            }
            string[] array = str.Split(new char[]
                {
            '.'
                });
            if (array.Length > 2 || array.Length < 1) {
                return false;
            }
            if (!ContainsOnlyCharacters(array[0], "-0123456789")) {
                return false;
            }
            if (array.Length == 2 && !ContainsOnlyCharacters(array[1], "0123456789")) {
                return false;
            }
            return true;
        }

        private static bool ContainsOnlyCharacters(string str, string allowedChars) {
            for (int i = 0; i < str.Length; i++) {
                if (!allowedChars.Contains(str[i])) {
                    return false;
                }
            }
            return true;
        }
    }
}
