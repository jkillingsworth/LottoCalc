using Android.Content;
using Android.Preferences;

namespace LottoCalc
{
    public static class Settings
    {
        private const string keySortResult = "SortResult";
        private const string keyUseZeroPad = "UseZeroPad";

        public static bool GetSortResult(Context context)
        {
            return PreferenceManager.GetDefaultSharedPreferences(context).GetBoolean(keySortResult, false);
        }

        public static bool GetUseZeroPad(Context context)
        {
            return PreferenceManager.GetDefaultSharedPreferences(context).GetBoolean(keyUseZeroPad, false);
        }
    }
}
