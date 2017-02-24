using Android.App;
using Android.OS;

namespace LottoCalc
{
    public class AboutDialogFragment : DialogFragment
    {
        public override Dialog OnCreateDialog(Bundle bundle)
        {
            var applicationName = Resources.GetString(Resource.String.ApplicationName);
            var aboutMessage = Resources.GetString(Resource.String.AboutMessage);
            var aboutVersion = Resources.GetString(Resource.String.AboutVersion);
            var buttonTextOK = Resources.GetString(Resource.String.ButtonTextOK);

            var packageName = Activity.PackageName;
            var packageInfo = Activity.PackageManager.GetPackageInfo(packageName, 0);
            var versionName = packageInfo.VersionName;

            return new AlertDialog.Builder(Activity)
                .SetIcon(Resource.Drawable.Icon)
                .SetTitle(applicationName)
                .SetMessage(string.Format("{0}\n\n{1} {2}", aboutMessage, aboutVersion, versionName))
                .SetPositiveButton(buttonTextOK, (s, ea) => { return; })
                .Create();
        }
    }
}
