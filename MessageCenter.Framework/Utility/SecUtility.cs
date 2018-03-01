using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

namespace MessageCenter.Framework.Utility
{
    public static class SecUtility
    {
        // Methods
        public static void CheckArrayParameter(ref string[] param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (param.Length < 1)
            {
                throw new ArgumentException(SR.GetString("Parameter_array_empty", new object[] { paramName }), paramName);
            }
            Hashtable hashtable = new Hashtable(param.Length);
            for (int i = param.Length - 1; i >= 0; i--)
            {
                CheckParameter(ref param[i], checkForNull, checkIfEmpty, checkForCommas, maxSize, paramName + "[ " + i.ToString(CultureInfo.InvariantCulture) + " ]");
                if (hashtable.Contains(param[i]))
                {
                    throw new ArgumentException(SR.GetString("Parameter_duplicate_array_element", new object[] { paramName }), paramName);
                }
                hashtable.Add(param[i], param[i]);
            }
        }

        public static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
        {
            if (param == null)
            {
                if (checkForNull)
                {
                    throw new ArgumentNullException(paramName);
                }
            }
            else
            {
                param = param.Trim();
                if (checkIfEmpty && (param.Length < 1))
                {
                    throw new ArgumentException(SR.GetString("Parameter_can_not_be_empty", new object[] { paramName }), paramName);
                }
                if ((maxSize > 0) && (param.Length > maxSize))
                {
                    throw new ArgumentException(SR.GetString("Parameter_too_long", new object[] { paramName, maxSize.ToString(CultureInfo.InvariantCulture) }), paramName);
                }
                if (checkForCommas && param.Contains(","))
                {
                    throw new ArgumentException(SR.GetString("Parameter_can_not_contain_comma", new object[] { paramName }), paramName);
                }
            }
        }

        public static void CheckPasswordParameter(ref string param, int maxSize, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (param.Length < 1)
            {
                throw new ArgumentException(SR.GetString("Parameter_can_not_be_empty", new object[] { paramName }), paramName);
            }
            if ((maxSize > 0) && (param.Length > maxSize))
            {
                throw new ArgumentException(SR.GetString("Parameter_too_long", new object[] { paramName, maxSize.ToString(CultureInfo.InvariantCulture) }), paramName);
            }
        }

        public static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize)
        {
            if (param == null)
            {
                return !checkForNull;
            }
            param = param.Trim();
            return (((!checkIfEmpty || (param.Length >= 1)) && ((maxSize <= 0) || (param.Length <= maxSize))) && (!checkForCommas || !param.Contains(",")));
        }

        public static bool ValidatePasswordParameter(ref string param, int maxSize)
        {
            if (param == null)
            {
                return false;
            }
            if (param.Length < 1)
            {
                return false;
            }
            if ((maxSize > 0) && (param.Length > maxSize))
            {
                return false;
            }
            return true;
        }
    }

}
