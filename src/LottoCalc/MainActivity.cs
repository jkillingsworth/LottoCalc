using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace LottoCalc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string keySelectedGame = "SelectedGame";
        private const string keyClearEnabled = "ClearEnabled";
        private const string keyResultString = "ResultString";

        private int selectedGamePosition = 0;

        private Spinner SpinnerGame
        {
            get { return FindViewById<Spinner>(Resource.Id.spinnerGame); }
        }

        private Button ButtonCompute
        {
            get { return FindViewById<Button>(Resource.Id.buttonCompute); }
        }

        private Button ButtonClear
        {
            get { return FindViewById<Button>(Resource.Id.buttonClear); }
        }

        private TextView TextviewResult
        {
            get { return FindViewById<TextView>(Resource.Id.textviewResult); }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Options, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menuitemOptionsAbout)
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
            ButtonClear.Enabled = savedInstanceState.GetBoolean(keyClearEnabled);
            TextviewResult.Text = savedInstanceState.GetString(keyResultString);

            base.OnRestoreInstanceState(savedInstanceState);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(keySelectedGame, selectedGamePosition);
            outState.PutBoolean(keyClearEnabled, ButtonClear.Enabled);
            outState.PutString(keyResultString, TextviewResult.Text);

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
            ButtonClear.Enabled = true;
            TextviewResult.Text = string.Join("  ", GetResult().Select(x => x.ToString()));
        }

        private void Clear()
        {
            ButtonClear.Enabled = false;
            TextviewResult.Text = "";
        }

        private List<int> GetResult()
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

            return pickValues;
        }
    }
}
