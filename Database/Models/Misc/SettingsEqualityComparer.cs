using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Database.Models.Misc
{
    public class SettingsEqualityComparer : IEqualityComparer<Settings>
    {
        public bool Equals([AllowNull] Settings x, [AllowNull] Settings y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.Name.Equals(y.Name, StringComparison.InvariantCulture) && x.Value.Equals(y.Value, StringComparison.InvariantCulture))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] Settings setting)
        {
            return setting.Name.GetHashCode();
        }
    }
}
