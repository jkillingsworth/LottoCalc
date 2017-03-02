using System;
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
        private readonly Game[] games = new Game[]
        {
            new Game("Powerball",     new CombinedPool(5, 1, 69), new CombinedPool(1, 1, 26)),
            new Game("Mega Millions", new CombinedPool(5, 1, 75), new CombinedPool(1, 1, 15)),
            new Game("Florida Lotto", new CombinedPool(6, 1, 53)),
            new Game("Cash4Life",     new CombinedPool(5, 1, 60), new CombinedPool(1, 1, 4)),
            new Game("Lucky Money",   new CombinedPool(4, 1, 47), new CombinedPool(1, 1, 17)),
            new Game("Fantasy 5",     new CombinedPool(5, 1, 36)),
            new Game("Pick 5",        new SeparatePool(5, 0, 9)),
            new Game("Pick 4",        new SeparatePool(4, 0, 9)),
            new Game("Pick 3",        new SeparatePool(3, 0, 9)),
            new Game("Pick 2",        new SeparatePool(2, 0, 9)),
        };

        private const string keySelectedGame = "SelectedGame";
        private const string keyResultEmpty = "ResultEmpty";
        private const string keyResultPrincipal = "ResultPrincipal";
        private const string keyResultSecondary = "ResultSecondary";
        private const string tagDialogAbout = "DialogAbout";

        private int selectedGamePosition = 0;
        private Result result = null;

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

            var adapterResource = Android.Resource.Layout.SimpleSpinnerDropDownItem;
            var adapter = new ArrayAdapter<string>(this, adapterResource);
            adapter.SetDropDownViewResource(adapterResource);

            foreach (var game in games)
            {
                adapter.Add(game.Name);
            }

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

            var resultEmpty = bundle.GetBoolean(keyResultEmpty);
            if (resultEmpty)
            {
                result = null;
            }
            else
            {
                result = new Result
                {
                    Principal = bundle.GetIntArray(keyResultPrincipal),
                    Secondary = bundle.GetIntArray(keyResultSecondary)
                };
            }

            base.OnRestoreInstanceState(bundle);
        }

        protected override void OnSaveInstanceState(Bundle bundle)
        {
            bundle.PutInt(keySelectedGame, selectedGamePosition);

            if (result == null)
            {
                bundle.PutBoolean(keyResultEmpty, true);
            }
            else
            {
                bundle.PutBoolean(keyResultEmpty, false);
                bundle.PutIntArray(keyResultPrincipal, result.Principal);
                bundle.PutIntArray(keyResultSecondary, result.Secondary);
            }

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
            result = Calculation.Execute(games[selectedGamePosition]);
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

            foreach (var value in GetValues(result.Principal))
            {
                var textview = new TextView(this);
                textview.Text = value;
                textview.SetTextSize(Android.Util.ComplexUnitType.Pt, 16);
                textview.SetBackgroundResource(Resource.Drawable.ResultBackground);
                textview.Gravity = GravityFlags.Center;
                LayoutResult.AddView(textview);
            }

            foreach (var value in GetValues(result.Secondary))
            {
                var textview = new TextView(this);
                textview.Text = value;
                textview.SetTextSize(Android.Util.ComplexUnitType.Pt, 16);
                textview.SetBackgroundResource(Resource.Drawable.ResultBackground);
                textview.Gravity = GravityFlags.Center;
                textview.SetTextColor(Android.Graphics.Color.Yellow);
                LayoutResult.AddView(textview);
            }
        }

        private string[] GetValues(int[] result)
        {
            var format = Settings.GetUseZeroPad(this) ? "00" : "0";
            var values = Settings.GetSortResult(this) ? result.OrderBy(x => x).ToArray() : result;
            var output = values.Select(x => x.ToString(format));

            return output.ToArray();
        }
    }
}
