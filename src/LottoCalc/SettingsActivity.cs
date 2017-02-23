using Android.App;
using Android.OS;
using Android.Preferences;

namespace LottoCalc
{
    [Activity(Icon = "@drawable/Icon", Label = "@string/LabelSettings")]
    public class SettingsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            FragmentManager
                .BeginTransaction()
                .Replace(Android.Resource.Id.Content, new SettingsFragment())
                .Commit();
        }
    }

    public class SettingsFragment : PreferenceFragment
    {
        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AddPreferencesFromResource(Resource.Xml.Settings);
        }
    }
}
