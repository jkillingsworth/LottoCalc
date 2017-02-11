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
        private const string keyResetEnabled = "ResetEnabled";
        private const string keyNumberString = "NumberString";

        private Button ButtonGenerate
        {
            get { return FindViewById<Button>(Resource.Id.buttonGenerate); }
        }

        private Button ButtonReset
        {
            get { return FindViewById<Button>(Resource.Id.buttonReset); }
        }

        private TextView TextNumbers
        {
            get { return FindViewById<TextView>(Resource.Id.textNumbers); }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            ButtonGenerate.Click += delegate { Generate(); };
            ButtonReset.Click += delegate { Reset(); };
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            ButtonReset.Enabled = savedInstanceState.GetBoolean(keyResetEnabled);
            TextNumbers.Text = savedInstanceState.GetString(keyNumberString);

            base.OnRestoreInstanceState(savedInstanceState);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(keyResetEnabled, ButtonReset.Enabled);
            outState.PutString(keyNumberString, TextNumbers.Text);

            base.OnSaveInstanceState(outState);
        }

        private void Generate()
        {
            ButtonReset.Enabled = true;
            TextNumbers.Text = string.Join("  ", GetNumbers().Select(x => x.ToString()));
        }

        private void Reset()
        {
            ButtonReset.Enabled = false;
            TextNumbers.Text = "";
        }

        private List<int> GetNumbers()
        {
            const int poolCount = 53;
            const int pickCount = 6;

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
