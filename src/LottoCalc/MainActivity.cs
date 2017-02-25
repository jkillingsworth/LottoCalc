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
        private const string tagDialogAbout = "DialogAbout";

        private int selectedGamePosition = 0;
        private int[] result = null;

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

        private LinearLayout LayoutResult
        {
            get { return FindViewById<LinearLayout>(Resource.Id.LayoutResult); }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Options, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item)
        {
            if (item.ItemId == Resource.Id.MenuitemOptionsSettings)
            {
                StartActivity(typeof(SettingsActivity));
            }

            if (item.ItemId == Resource.Id.MenuitemOptionsAbout)
            {
                new AboutDialogFragment().Show(FragmentManager, tagDialogAbout);
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

        protected override void OnResume()
        {
            base.OnResume();

            RefreshScreen();
        }

        protected override void OnRestoreInstanceState(Bundle bundle)
        {
            selectedGamePosition = bundle.GetInt(keySelectedGame);
            result = bundle.GetIntArray(keyResultValues);

            base.OnRestoreInstanceState(bundle);
        }

        protected override void OnSaveInstanceState(Bundle bundle)
        {
            bundle.PutInt(keySelectedGame, selectedGamePosition);
            bundle.PutIntArray(keyResultValues, result);

            base.OnSaveInstanceState(bundle);
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
            Toast.MakeText(this, Resource.String.ToastResultCleared, ToastLength.Short).Show();
        }

        private void Compute()
        {
            result = GetResult();
            RefreshScreen();
        }

        private void Clear()
        {
            result = null;
            RefreshScreen();
        }

        private void RefreshScreen()
        {
            ButtonClear.Enabled = (result != null);

            LayoutResult.RemoveAllViews();

            if (result == null)
            {
                return;
            }

            var values = Settings.GetSortResult(this) ? result.OrderBy(x => x).ToArray() : result;
            var format = Settings.GetUseZeroPad(this) ? "00" : "0";

            foreach (var value in values)
            {
                var textview = new TextView(this);
                textview.Text = value.ToString(format);
                textview.SetTextSize(Android.Util.ComplexUnitType.Pt, 16);
                textview.SetBackgroundResource(Resource.Drawable.ResultBackground);
                textview.Gravity = GravityFlags.Center;
                LayoutResult.AddView(textview);
            }
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
