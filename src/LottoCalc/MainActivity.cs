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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var buttonGenerate = FindViewById<Button>(Resource.Id.buttonGenerate);
            var buttonReset = FindViewById<Button>(Resource.Id.buttonReset);

            buttonGenerate.Click += delegate { Generate(); };
            buttonReset.Click += delegate { Reset(); };
        }

        private void Generate()
        {
            var textNumbers = FindViewById<TextView>(Resource.Id.textNumbers);
            var buttonReset = FindViewById<Button>(Resource.Id.buttonReset);

            textNumbers.Text = string.Join("  ", GetNumbers().Select(x => x.ToString()));
            buttonReset.Enabled = true;
        }

        private void Reset()
        {
            var textNumbers = FindViewById<TextView>(Resource.Id.textNumbers);
            var buttonReset = FindViewById<Button>(Resource.Id.buttonReset);

            textNumbers.Text = "";
            buttonReset.Enabled = false;
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
