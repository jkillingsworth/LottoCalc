using Android.Content;
using Android.Preferences;

namespace LottoCalc
{
    public static class Settings
    {
        private const string keySortResult = "SortResult";

        public static bool GetSortResult(Context context)
        {
            return PreferenceManager.GetDefaultSharedPreferences(context).GetBoolean(keySortResult, false);
        }
    }
}
