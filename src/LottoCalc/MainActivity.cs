using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
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
            ButtonCompute.Click += delegate { Compute(); };
            ButtonClear.Click += delegate { Clear(); };
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
