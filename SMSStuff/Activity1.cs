using Android.App;
using Android.Content;
using Android.Telephony;
using Android.Widget;
using Android.OS;

namespace SMSStuff
{
    [Activity(Label = "SMSStuff", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        private SmsManager _smsManager;
        private BroadcastReceiver _smsSentBroadcastReceiver, _smsDeliveredBroadcastReceiver;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var sendSMSButton = FindViewById<Button>(Resource.Id.MyButton);
            var phoneNumberEditText = FindViewById<EditText>(Resource.Id.editText1);
            var messageEditText = FindViewById<EditText>(Resource.Id.editText2);
            _smsManager = SmsManager.Default;

            sendSMSButton.Click += (s, e) =>
            {
                var phone = phoneNumberEditText.Text;
                var message = messageEditText.Text;

                if (!PhoneNumberUtils.IsWellFormedSmsAddress(phone) || PhoneNumberUtils.IsEmergencyNumber(phone)) return;

                var piSent = PendingIntent.GetBroadcast(this, 0, new Intent("SMS_SENT"), 0);
                var piDelivered = PendingIntent.GetBroadcast(this, 0, new Intent("SMS_DELIVERED"), 0);

                _smsManager.SendTextMessage(phone, null, message, piSent, piDelivered);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            _smsSentBroadcastReceiver = new SMSSentReceiver();
            _smsDeliveredBroadcastReceiver = new SMSDeliveredReceiver();

            RegisterReceiver(_smsSentBroadcastReceiver, new IntentFilter("SMS_SENT"));
            RegisterReceiver(_smsDeliveredBroadcastReceiver, new IntentFilter("SMS_DELIVERED"));
        }

        protected override void OnPause()
        {
            base.OnPause();

            UnregisterReceiver(_smsSentBroadcastReceiver);
            UnregisterReceiver(_smsDeliveredBroadcastReceiver);
        }
    }

    [BroadcastReceiver(Exported = true, Permission = "//receiver/@android:android.permission.SEND_SMS")]
    public class SMSSentReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            switch ((int)ResultCode)
            {
                case (int)Result.Ok:
                    Toast.MakeText(Application.Context, "SMS has been sent", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.GenericFailure:
                    Toast.MakeText(Application.Context, "Generic Failure", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.NoService:
                    Toast.MakeText(Application.Context, "No Service", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.NullPdu:
                    Toast.MakeText(Application.Context, "Null PDU", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.RadioOff:
                    Toast.MakeText(Application.Context, "Radio Off", ToastLength.Short).Show();
                    break;
            }
        }
    }

    [BroadcastReceiver(Exported = true, Permission = "//receiver/@android:android.permission.SEND_SMS")]
    public class SMSDeliveredReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            switch ((int)ResultCode)
            {
                case (int)Result.Ok:
                    Toast.MakeText(Application.Context, "SMS Delivered", ToastLength.Short).Show();
                    break;
                case (int)Result.Canceled:
                    Toast.MakeText(Application.Context, "SMS not delivered", ToastLength.Short).Show();
                    break;
            }
        }
    }
}

