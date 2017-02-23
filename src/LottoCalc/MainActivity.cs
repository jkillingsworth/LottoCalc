using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace LottoCalc
{
    [Activity(MainLauncher = true, Icon = "@drawable/Icon")]
    public class MainActivity : Activity
    {
        private const string keySelectedGame = "SelectedGame";
        private const string keyResultValues = "ResultValues";
        private const string keyDoResultSort = "DoResultSort";

        private int selectedGamePosition = 0;
        private int[] result = null;
        private bool resultSort = false;

        private Spinner SpinnerGame
        {
            get { return FindViewById<Spinner>(Resource.Id.SpinnerGame); }
        }

        private Button ButtonCompute
        {
            get { return FindViewById<Button>(Resource.Id.ButtonCompute); }
        }

        private Button ButtonClear
        {
            get { return FindViewById<Button>(Resource.Id.ButtonClear); }
        }

        private TextView TextviewResult
        {
            get { return FindViewById<TextView>(Resource.Id.TextviewResult); }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Options, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.FindItem(Resource.Id.MenuitemOptionsSortResult).SetChecked(resultSort);

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            if (item.ItemId == Resource.Id.MenuitemOptionsSortResult)
            {
                resultSort = !resultSort;
                item.SetChecked(resultSort);
                TextviewResult.Text = GetResultString();
            }

            if (item.ItemId == Resource.Id.MenuitemOptionsAbout)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;

                var applicationName = Resources.GetString(Resource.String.ApplicationName);
                var aboutTitle = Resources.GetString(Resource.String.AboutTitle);
                var aboutMessage = Resources.GetString(Resource.String.AboutMessage);
                var aboutVersion = Resources.GetString(Resource.String.AboutVersion);
                var acceptButtonText = Resources.GetString(Resource.String.AcceptButtonText);

                new AlertDialog.Builder(this)
                    .SetIcon(Resource.Drawable.Icon)
                    .SetTitle(aboutTitle)
                    .SetMessage(string.Format("{0}\n\n{1}\n\n{2} {3}", applicationName, aboutMessage, aboutVersion, version))
                    .SetNeutralButton(acceptButtonText, (s, ea) => { return; })
                    .Create().Show();
            }

            return base.OnMenuItemSelected(featureId, item);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            adapter.Add("Florida Lotto");
            adapter.Add("Fantasy 5");

            SpinnerGame.Adapter = adapter;
            SpinnerGame.ItemSelected += SpinnerGame_ItemSelected;
            ButtonCompute.Click += ButtonCompute_Click;
            ButtonClear.Click += ButtonClear_Click;
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            selectedGamePosition = savedInstanceState.GetInt(keySelectedGame);
            result = savedInstanceState.GetIntArray(keyResultValues);
            resultSort = savedInstanceState.GetBoolean(keyDoResultSort);

            ButtonClear.Enabled = (result != null);
            TextviewResult.Text = GetResultString();

            base.OnRestoreInstanceState(savedInstanceState);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(keySelectedGame, selectedGamePosition);
            outState.PutIntArray(keyResultValues, result);
            outState.PutBoolean(keyDoResultSort, resultSort);

            base.OnSaveInstanceState(outState);
        }

        private void SpinnerGame_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (selectedGamePosition != e.Position)
            {
                selectedGamePosition = e.Position;
                Clear();
            }
        }

        private void ButtonCompute_Click(object sender, EventArgs e)
        {
            Compute();
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            Clear();
            Toast.MakeText(this, Resource.String.ToastResultWasCleared, ToastLength.Short).Show();
        }

        private void Compute()
        {
            result = GetResult();

            ButtonClear.Enabled = (result != null);
            TextviewResult.Text = GetResultString();
        }

        private void Clear()
        {
            result = null;

            ButtonClear.Enabled = (result != null);
            TextviewResult.Text = GetResultString();
        }

        private string GetResultString()
        {
            if (result == null)
            {
                return string.Empty;
            }

            var values = resultSort
                ? result.OrderBy(x => x).ToArray()
                : result;

            return string.Join("  ", values.Select(x => x.ToString()));
        }

        private int[] GetResult()
        {
            int poolCount;
            int pickCount;

            switch (selectedGamePosition)
            {
                case 0:
                    poolCount = 53;
                    pickCount = 6;
                    break;

                case 1:
                    poolCount = 36;
                    pickCount = 5;
                    break;

                default:
                    throw new ApplicationException();
            }

            var poolValues = new List<int>(poolCount);
            var pickValues = new List<int>(pickCount);

            var random = new Random();

            for (int i = 0; i < poolCount; i++)
            {
                poolValues.Add(i + 1);
            }

            for (int i = 0; i < pickCount; i++)
            {
                var index = random.Next(poolValues.Count);
                var value = poolValues[index];
                poolValues.RemoveAt(index);
                pickValues.Add(value);
            }

            return pickValues.ToArray();
        }
    }
}
