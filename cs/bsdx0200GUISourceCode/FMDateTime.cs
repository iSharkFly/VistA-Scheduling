/*
 * Copyright (C) 2004-2009  Medsphere Systems Corporation
 * All rights reserved.
 *
 * This source code contains the intellectual property
 * of its copyright holder(s), and is made available
 * under a license. If you do not know the terms of
 * the license, please stop and do not read further.
 *
 * Please read LICENSES for detailed information about 
 * the license this source code file is available under. 
 * Questions should be directed to legal@medsphere.com
 *
 *
 * Licensed under AGPL v3.
 * 
 * 

 Mods by Sam Habiel to use in Scheduling GUI.
 */


using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IndianHealthService.ClinicalScheduling
{
	public delegate void FMDateTimeHandler (FMDateTime time);
	public delegate string FMDateStringHandler (string s);
	
	[Serializable]
	[System.Xml.Serialization.SoapType (Namespace="http://ws.medsphere.com")]
	public class FMDateTime : IComparable, ICloneable
	{
		/* public properties */
		[System.Xml.Serialization.SoapAttribute (Namespace="http://ws.medsphere.com")]
		public DateTime DateTime {
			get {
				switch (Precision) {
				case FMDateTimePrecision.YearOnly:
					return new DateTime (Year, 1, 1);
				case FMDateTimePrecision.YearMonthOnly:
					return new DateTime (Year, Month, 1);
				case FMDateTimePrecision.DateOnly:
					return new DateTime (Year, Month, Day);
				default:
					return new DateTime (Year, Month, Day, Hour, Minute, Second);
				}
			}
		}

		public int Year {
			get { return year; }
			set { year = value; }
		}

		public int Month {
			get { return month; }
			set { month = value; }
		}

		public int Day {
			get { return day; }
			set { day = value; }
		}

		public int Hour {
			get { return hour; }
			set {
				if (value > 24) {
					throw new ArgumentException ("Hour cannot be greater than 24");
				}

				hour = value % 24;
			}
		}

		public int Minute {
			get { return minute; }
			set {
				if (value > 59) {
					throw new ArgumentException ("Minute cannot be greater than 59");
				}

				minute = value;
			}
		}

		public int Second {
			get { return second; }
			set {
				if (value > 59) {
					throw new ArgumentException ("Second cannot be greater than 59");
				}

				second = value;
			}
		}

		public static FMDateTime ServerNow {
			get { return FMDateTime.Create (DateTime.Now - server_offset); }
		}

		public string RelativeFMDateString {
			get { return Raw; }
		}

		public string ShortHandString {
			get {
				if (Raw != null && date_format.IsMatch (Raw.Replace (" ", ""))) {
					return Raw;
				}
				return ToString ();
			}
		}

		public FMDateTime AtZero {
			get {
				FMDateTime d = (FMDateTime)this.Clone ();
				d.Precision = FMDateTimePrecision.DateAndTime;
				d.Hour = 0;
				d.Minute = 0;
				d.Second = 0;
				return d;
			}
		}

		public FMDateTime AtMidnight {
			get {
				FMDateTime d = (FMDateTime)this.Clone ();
				d.Precision = FMDateTimePrecision.DateAndTime;
				d.Hour = 23;
				d.Minute = 59;
				d.Second = 59;
				return d;
			}
		}

		public FMDateTime DateOnly {
			get {
				FMDateTime d = (FMDateTime)this.Clone ();
				if (Precision != FMDateTimePrecision.DateAndTime) {
					return d;
				}
				d.Precision = FMDateTimePrecision.DateOnly;
				d.Hour = d.Minute = d.Second = 0;
				return d;
			}
		}

		public string FMDateString {
			get {
				switch (Precision) {
				case FMDateTimePrecision.YearOnly:
					return String.Format ("{0:000}", year - 1700);
				case FMDateTimePrecision.YearMonthOnly:
					return String.Format ("{0:000}{1:00}", year - 1700, month);
				case FMDateTimePrecision.DateOnly:
					return String.Format ("{0:000}{1:00}{2:00}", year - 1700, month, day);
				case FMDateTimePrecision.DateAndTime:
				default:
					if (second > 0) {
						return String.Format ("{0:000}{1:00}{2:00}.{3:00}{4:00}{5:00}",
								      year - 1700, month, day, hour, minute, second);
					} else {
						return String.Format ("{0:000}{1:00}{2:00}.{3:00}{4:00}",
								      year - 1700, month, day, hour, minute);
					}
				}
			}
		}

		/* public fields */
		public FMDateTimePrecision Precision;

		[NonSerialized]
		public static FMDateStringHandler ValidationMethod;

		public static FMDateTime MinValue; // 1/1/1700
		public static FMDateTime MaxValue; // 12/31/2699 12:59 PM

		/* public methods */
		static FMDateTime ()
		{
			// This is the equivalent of the FMDateTime string '0000101'
			// We do this manually to avoid parsing overhead here.
			MinValue = new FMDateTime (); // 1/1/1700
			MinValue.Hour = MinValue.Minute = MinValue.Second = 0;
			MinValue.Day = MinValue.Month = 1;
			MinValue.Year = 1700;
			MinValue.Precision = FMDateTimePrecision.DateOnly;

			// This is the equivalent of the FMDateTime string '9991231.235959'
			// We do this manually to avoid parsing overhead here.
			MaxValue = new FMDateTime (); // 12/31/2699 12:59 PM
			MaxValue.Hour = 23;
			MaxValue.Minute = 59;
			MaxValue.Second = 59;
			MaxValue.Day = 31;
			MaxValue.Month = 12;
			MaxValue.Year = 2699;
			MaxValue.Precision = FMDateTimePrecision.DateAndTime;
		}

		public FMDateTime ()
		{
		}

        public FMDateTime(DateTime d)
            : this(d, FMDateTimePrecision.DateAndTime)
        {
        }

        public FMDateTime(DateTime d, FMDateTimePrecision precision)
        {
            if (d > MaxValue.DateTime || d < MinValue.DateTime)
                return;

            this.Precision = precision;
            this.Year = d.Year;
            this.Month = d.Month;
            this.Day = d.Day;
            this.Hour = d.Hour;
            this.Minute = d.Minute;
            this.Second = d.Second;
        }
		
		public static FMDateTime Create (DateTime d, FMDateTimePrecision precision)
		{
			if (d > MaxValue.DateTime || d < MinValue.DateTime) {
				return null;
			}
			
			FMDateTime f = new FMDateTime ();
			f.Precision = precision;
			f.Year = d.Year;
			f.Month = d.Month;
			f.Day = d.Day;
			f.Hour = d.Hour;
			f.Minute = d.Minute;
			f.Second = d.Second;

			return f;
		}

		public static FMDateTime Create (DateTime d)
		{
			return Create (d, FMDateTimePrecision.DateAndTime);
		}

		public static FMDateTime Parse (string str)
		{
			return Parse (str, FMDateTime.ValidationMethod);
		}

		public static FMDateTime Parse (string str,
						FMDateStringHandler validation_method)
		{
			if (validation_method == null) {
				throw new ArgumentNullException ("You must pass in a valid validation_method");
			}

			if (str == null) {
				return null;
			}

			FMDateTime date = null;

			str = str.Trim ();
			if (str == "0" || str == String.Empty) {
				return null;
			}

			if (str.IndexOf ("@") != -1) {
				date = new FMDateTime ();

				// string has a date and time part
				string[] tokens = str.Split (new char[] {'@'}, 2);
				if (ParseDatePart (tokens[0], ref date)
 				    || ParseUsingDateTime (tokens[0], ref date)
				    || (validation_method != null
				        && ParseInternalFormat (validation_method (tokens[0]), ref date))) {
					// Its been decided that if you have an
					// invalid time part, that the entire
					// string is invalid
					if (!ParseTimePart (tokens[1], true, ref date)) {
						return null;
					}

					date.Raw = str;
					return date;
				} else {
					// Account for @0600
					date = FMDateTime.ServerNow;
					if (!ParseTimePart (tokens[1], true, ref date)) {
						return null;
					}
					return date;
				}
			}
			
			if (ParseDatePart (str, ref date)) {
				date.Raw = str;
				return date;
			}

			if (ParseTimePart (str, false, ref date)) {
				FMDateTime now = ServerNow;
				date.Year = now.Year;
				date.Month = now.Month;
				date.Day = now.Day;
				return date;
			}

			if (ParseUsingDateTime (str, ref date)) {
				return date;
			}

			if (ParseInternalFormat (str, ref date)) {
				return date;
			}

			if (validation_method != null) {
				if (ParseInternalFormat (validation_method (str), ref date)) {
					return date;
				}
				return null;
			}

			if (date == null) {
				Console.WriteLine ("WARNING: FMDateTime failed parsing '{0}'", str);
			}

			return date;
		}

		public static FMDateTime Parse (string str, FMDateTimePrecision precision)
		{
			FMDateTime date = FMDateTime.Parse (str);
			if (date != null) {
				date.Precision = precision;
			}

			return date;
		}

		public void PopulateFrom12HrTime (int hour, int minute, int second, bool is_pm)
		{
			if (hour < 12 && is_pm) {
				hour += 12;
			} else if (hour == 12 && !is_pm) {
				hour = 0;
			}

			Hour = hour;
			Minute = minute;
			Second = second;
		}

		public bool IsFutureDate
		{
			get {
				return (CompareTo (Precision, FMDateTime.ServerNow, FMDateTime.ServerNow.Precision) > 0);
			}
		}

		public bool IsPastDate
		{
			get {
				return (CompareTo (Precision, FMDateTime.ServerNow, FMDateTime.ServerNow.Precision) < 0);
			}
		}

		public static void UpdateServerNow (FMDateTime server_now)
		{
			if (server_now != null) {
				server_offset = (DateTime.Now - server_now.DateTime);
			}
		}

		public override string ToString ()
		{
			switch (Precision) {
			case FMDateTimePrecision.YearOnly:
				return DateTime.ToString ("yyyy");
			case FMDateTimePrecision.YearMonthOnly:
				return DateTime.ToString ("Y");
			case FMDateTimePrecision.DateOnly:
				return DateTime.ToString ("d");
			default:
				return DateTime.ToString ("G");
			}
		}

		public static string ToString (FMDateTime date)
		{
			if (date != null) {
				return date.ToString ();
			}
			return String.Empty;
		}

		public string ToString (string format)
		{
			return DateTime.ToString (format);
		}

		public static string ToString (FMDateTime date, string format)
		{
			if (date != null) {
				return date.ToString (format);
			}
			return String.Empty;
		}

		public string ToDateString ()
		{
			return DateTime.ToString ("d");
		}

		public static string ToDateString (FMDateTime date)
		{
			if (date != null) {
				return date.ToDateString ();
			}
			return String.Empty;
		}

		public string ToTimeString ()
		{
			return DateTime.ToString ("t");
		}

		public static string ToTimeString (FMDateTime date)
		{
			if (date != null) {
				return date.ToTimeString ();
			}
			return String.Empty;
		}

		public static string ToFMDateString (FMDateTime date)
		{
			if (date != null) {
				return date.FMDateString;
			}
			return String.Empty;
		}
				
		/**
		 * Compares this FMDateTime instance with given FMDateTimePrecision this_p to dt
		 * using the given FMDateTimePrecision p.
		 */ 
		public int CompareTo (FMDateTimePrecision this_p, FMDateTime dt, FMDateTimePrecision dt_p)
		{
			int r;
			FMDateTimePrecision save_this_p = Precision;
			FMDateTimePrecision save_dt_p = dt.Precision;
			Precision = this_p;
			dt.Precision = dt_p;
			r = DateTime.CompareTo (dt.DateTime);
			Precision = save_this_p;
			dt.Precision = save_dt_p;
			return r;
		}

		/**
		 * Implementation of IComparable interface.
		 */
		public int CompareTo (object o)
		{
			if (o == null) {
				return 1;
			} else if (o is FMDateTime) {
				FMDateTime f = (FMDateTime)o;
				int r = DateTime.CompareTo (f.DateTime);
				if (r == 0) {
					// special cases of DateTime comparison:
					//     1900 and January,1900 and 01/01/1900 are all 
					//         represented as 01/01/1900
					//     TODAY@0 and TODAY are both represented as TODAY@0
					// these are the cases where precision has the last word
					if (Precision < f.Precision) {
						r = -1;
					} else if (Precision > f.Precision) {
						r = 1;
					}
				}
				return r;
			} else if (o is DateTime) {
				DateTime d = (DateTime)o;
				return DateTime.CompareTo (d);
			}
			
			throw new ArgumentException ("Value is not a DateTime or FMDateTime.");
		}

		public static int Compare (FMDateTime a, FMDateTime b)
		{
			if (a == null && b == null) {
				return 0;
			} else if (a != null && b != null) {
				return a.CompareTo (b);
			/* We sort the non-null item before the null one for the mixed case */
			} else if (a == null) {
				return -1;
			} else if (b == null) {
				return 1;
			}

			// This code path really has no way of being hit.
			return 0;
		}

		public override bool Equals (object o)
		{
			if (o == null) {
				return false;
			} else if (o is FMDateTime) {
				FMDateTime f = (FMDateTime)o;

				if (f.Precision != Precision) {
					return false;
				}

				switch (Precision) {
				case FMDateTimePrecision.YearOnly:
					return Year == f.Year;
				case FMDateTimePrecision.YearMonthOnly:
					return Year == f.Year && Month == f.Month;
				case FMDateTimePrecision.DateOnly:
					return Year == f.Year && Month == f.Month && Day == f.Day;
				case FMDateTimePrecision.DateAndTime:
				default:
					return Year == f.Year && Month == f.Month && Day == f.Day
					       && Hour == f.Hour && Minute == f.Minute && Second == f.Second;
				}
			}

			throw new ArgumentException ("Value is not a FMDateTime.");
		}

		public override int GetHashCode ()
		{
			return (int)Precision + year + month + day + hour + minute + second;
		}

		/**
		 * This gets a hash code based upon the FMDateTime precision, so that
		 * an object can be stored based on DateOnly, for example, and if you
		 * try to look it up later using a different FMDateTime object that
		 * has the same date, but may have different time.  Seconds are
		 * intentionally never factored into this hash code, even for DateAndTime
		 * cases.  If you want to factor in seconds as  well, just use GetHashCode().
		 *
		 * @return   An integer hash code.
		 */
		public int GetPrecisionAwareHashCode ()
		{
			int hash_code = (int)Precision;

			switch (Precision)
			{
			case FMDateTimePrecision.YearOnly:
				hash_code += year;
				break;
			case FMDateTimePrecision.YearMonthOnly:
				hash_code += year;
				hash_code += month;
				break;
			case FMDateTimePrecision.DateOnly:
				hash_code += year;
				hash_code += month;
				hash_code += day;
				break;
			case FMDateTimePrecision.DateAndTime:
			default:
				hash_code += year;
				hash_code += month;
				hash_code += day;
				hash_code += hour;
				hash_code += minute;
				break;
			}

			return hash_code;
		}

		public object Clone ()
		{
			FMDateTime d = new FMDateTime ();
			d.Precision = Precision;
			d.Year = year;
			d.Month = month;
			d.Day = day;
			d.Hour = hour;
			d.Minute = minute;
			d.Second = second;
			return d;
		}
	
		public static bool operator == (FMDateTime a, FMDateTime b)
		{
			object obj_a = (object)a;
			object obj_b = (object)b;

			if (obj_a == null && obj_b == null) {
				return true;
			} else if (obj_a != null && obj_b != null) {
				return a.Equals (b);
			} else {
				return false;
			}
		}

		public static bool operator != (FMDateTime a, FMDateTime b)
		{
			return !(a == b);
		}

		public static bool operator > (FMDateTime a, FMDateTime b)
		{
			if (a == null) {
				throw new ArgumentException ("Left hand argument to comparison cannot be null.");
			}

			return (a.CompareTo (b) > 0);
		}

		public static bool operator >= (FMDateTime a, FMDateTime b)
		{
			if (a == null) {
				throw new ArgumentException ("Left hand argument to comparison cannot be null.");
			}

			return (a.CompareTo (b) >= 0);
		}

		public static bool operator < (FMDateTime a, FMDateTime b)
		{
			if (a == null) {
				throw new ArgumentException ("Left hand argument to comparison cannot be null.");
			}

			return (a.CompareTo (b) < 0);
		}

		public static bool operator <= (FMDateTime a, FMDateTime b)
		{
			if (a == null) {
				throw new ArgumentException ("Left hand argument to comparison cannot be null.");
			}

			return (a.CompareTo (b) <= 0);
		}

		public static implicit operator FMDateTime (DateTime d)
		{
			return FMDateTime.Create (d);
		}

		/* protected properties */
		protected string Raw;
		
		/* private properties */
		private static Calendar CurrentCalendar {
			get { return CultureInfo.CurrentCulture.Calendar; }
		}

		/* private fields */
		private int year, month, day, hour, minute, second;

		// We do this here so we can lazy load the compiled regexes.
		private static Regex internal_format {
			get {
				if (priv_internal_format == null) {
					priv_internal_format = new Regex (@"^(\d{3})(\d{2})?(\d{2})?(\.(\d{1,2})?(\d{1,2})?(\d{1,2})?)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				}
				
				return priv_internal_format;
			}
		}
		private static Regex priv_internal_format;

		private static Regex date_format {
			get {
				if (priv_date_format == null) {
					priv_date_format = new Regex (@"^(t(oday)?|n(ow)?)(([+-])(\d+)(w)?)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				}
				
				return priv_date_format;
			}
		}
		private static Regex priv_date_format;

		private static Regex time_format {
			get {
				if (priv_time_format == null) {
					priv_time_format = new Regex (@"^(\d{1,2})(:(\d{2}))?(:(\d{2}))?(\s*)(AM|PM)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				}
				
				return priv_time_format;
			}
		}
		private static Regex priv_time_format;

		private static TimeSpan server_offset = new TimeSpan ();

		/* private methods */
		private static bool ParseDatePart (string str, ref FMDateTime date)
		{
			FMDateTime orig_date = date;
		
			string clean = str.Replace (" ", "");

			// see if it matches (t|today|now) +/- some number
			if (!date_format.IsMatch (clean)) {
				return false;
			}

			Match m = date_format.Match (clean);
			if (m != null) {
				if (date == null) {
					date = new FMDateTime ();
				}

				// be safe about dates like T-99999999
				try {
					int modifier = 0;
					if (m.Groups[5].Value != String.Empty) {
						int sign_bit = 1;
						if (m.Groups[5].Value == "-") {
							sign_bit = -1;
						}
						
						// Convert will bomb if the modifier
						// won't fit into an int (it's invalid
						// anyway)
						modifier = sign_bit * Convert.ToInt32 (m.Groups[6].Value);
					}

					DateTime dt = ServerNow.DateTime;
					if (m.Groups[7].Value.ToLower () == "w") {
						dt = CurrentCalendar.AddWeeks (dt, modifier);
					} else {
						dt = CurrentCalendar.AddDays (dt, modifier);
					}

					date.Day = dt.Day;
					date.Month = dt.Month;
					date.Year = dt.Year;

					if (m.Groups[1].Value.ToLower ().StartsWith ("n")) {
						date.Precision = FMDateTimePrecision.DateAndTime;
						date.Hour = dt.Hour;
						date.Minute = dt.Minute;
						date.Second = dt.Second;
					} else {
						date.Precision = FMDateTimePrecision.DateOnly;
						date.Hour = date.Minute = date.Second = 0;
					}
				} catch {
					date = orig_date;
					return false;
				}

				return true;
			}

			date = orig_date;
			return false;
		}

		private static bool ParseTimePart (string str, bool try_int_parse, ref FMDateTime date)
		{
            int time;
			str = str.ToUpper ();
			if (str == "NOON") {
				if (date == null) {
					date = new FMDateTime ();
				}
			
				date.Hour = 12;
				date.Minute = date.Second = 0;
				
				date.Precision = FMDateTimePrecision.DateAndTime;

				return true;
			} else if (str == "MID" || str == "MIDNIGHT") {
				if (date == null) {
					date = new FMDateTime ();
				}
			
				date.Hour = 23;
				date.Minute = 59;
				date.Second = 59;

				date.Precision = FMDateTimePrecision.DateAndTime;

				return true;
			} else if (time_format.IsMatch (str)) {
				Match m = time_format.Match (str);
				if (m == null) {
					return false;
				}

				int hour, minute, second;
                int.TryParse(m.Groups[1].Value, out hour);
				int.TryParse(m.Groups[3].Value, out minute);
				int.TryParse(m.Groups[5].Value, out second);

				if (m.Groups[7].Value == "PM") {
					hour += 12;
				}

				if (hour == 24 && minute == 0 && second == 0) {
					hour = 23;
					minute = second = 59;
				}

				if (!ValidateTime (hour, minute, second)) {
					return false;
				}

				if (date == null) {
					date = new FMDateTime ();
				}

				date.Hour = hour;
				date.Minute = minute;
				date.Second = second;
				date.Precision = FMDateTimePrecision.DateAndTime;

				return true;
			} else if (try_int_parse && int.TryParse(str, out time)) {

				int hour, minute, second;
				if (time <= 2359) {
					hour = time / 100;
					minute = time - (hour * 100);
					second = 0;
				} else if (time <= 235959) {
					hour = time / 10000;
					minute = (time - (hour * 10000)) / 100;
					second = time - ((time / 100) * 100);
				} else {
					return false;
				}
				
				if (hour == 24 && minute == 0 && second == 0) {
					hour = 23;
					minute = second = 59;
				}

				if (!ValidateTime (hour, minute, second)) {
					return false;
				}

				if (date == null) {
					date = new FMDateTime ();
				}
				
				date.Hour = hour;
				date.Minute = minute;
				date.Second = second;
				date.Precision = FMDateTimePrecision.DateAndTime;

				return true;
			}
			
			return false;
		}

		private static bool ParseUsingDateTime (string str, ref FMDateTime date)
		{
			// we need to use DateTime.Parse to allow
			// roundtripping of our ToString () methods

			// LAMESPEC: There isn't any way to find out whether a
			// DateTime contains a time part, or just a date part
			// after calling Parse, so we have to specifically call
			// ParseExact on a few known formats
			try {
				string[] formats = new string[] {
					"yyyy"
				};
				
				DateTime d = DateTime.ParseExact (str, formats, null,
				                                  DateTimeStyles.AllowWhiteSpaces);
				if (date == null) {
					date = new FMDateTime ();
				}
				                                  
				date.Year = d.Year;
				date.Precision = FMDateTimePrecision.YearOnly;
				return true;
			} catch (FormatException) {
			}

			try {
				string[] formats = new string[] {
					"Y"
				};
				
				DateTime d = DateTime.ParseExact (str, formats, null,
				                                  DateTimeStyles.AllowWhiteSpaces);
				if (date == null) {
					date = new FMDateTime ();
				}
				
				date.Year = d.Year;
				date.Month = d.Month;
				date.Precision = FMDateTimePrecision.YearMonthOnly;
				return true;
			} catch (FormatException) {
			}

			try {
				string[] formats = new string[] {
					"d", "MM/dd/yy"
				};
				
				DateTime d = DateTime.ParseExact (str, formats, null,
				                                  DateTimeStyles.AllowWhiteSpaces);
				if (date == null) {
					date = new FMDateTime ();
				}
				
				date.Year = d.Year;
				date.Month = d.Month;
				date.Day = d.Day;
				date.Precision = FMDateTimePrecision.DateOnly;
				return true;
			} catch (FormatException) {
			}

			try {
				string[] formats = new string[] {
					"g", "G", "MM/dd/yy hh:mm tt",
					"MM/dd/yy h:mm tt"
				};
				
				DateTime d = DateTime.ParseExact (str, formats, null,
				                                  DateTimeStyles.AllowWhiteSpaces);
				if (date == null) {
					date = new FMDateTime ();
				}
				
				date.Year = d.Year;
				date.Month = d.Month;
				date.Day = d.Day;

				date.Hour = d.Hour;
				date.Minute = d.Minute;
				date.Second = d.Second;

				date.Precision = FMDateTimePrecision.DateAndTime;
				return true;
			} catch (FormatException) {
			}
		
			/* XXX: Disabiling this for now, since it sucks incredibly
			// first try parsing date & time
			try {
				string[] date_time_formats = new string[] {
					"dddd*, MMMM* dd, yyyy HH*:mm* tt*", "f",
					"dddd*, MMMM* dd, yyyy HH*:mm*:ss* tt*", "F",
					"MM/dd/yyyy HH*:mm* tt*", "g",
					"MM/dd/yyyy HH*:mm*:ss* tt*", "G",
					"dddd*, MMMM* dd, yyyy HH*:mm*:ss* tt*", "U"
				};

				DateTime d = DateTime.ParseExact (str, date_time_formats, null,
				                                  DateTimeStyles.AllowWhiteSpaces);
				date.Year = d.Year;
				date.Month = d.Month;
				date.Day = d.Day;
				date.Hour = d.Hour;
				date.Minute = d.Minute;
				date.Second = d.Second;
				date.Precision = FMDateTimePrecision.DateAndTime;
				return true;
			} catch { }
			
			// fall back on just parsing a date
			try {
				string[] date_formats = new string[] {
					"d", "D", "m", "M", "y", "Y"
				};

				DateTime d = DateTime.ParseExact (str, date_formats, null,
				                                  DateTimeStyles.AllowWhiteSpaces);
				date.Year = d.Year;
				date.Month = d.Month;
				date.Day = d.Day;
				date.Precision = FMDateTimePrecision.DateOnly;
				return true;
			} catch { }
			
			// if nothing else, try a couple of time formats
			try {
				string[] time_formats = new string[] {
					"HH*:mm* tt*", "t",
					"HH*:mm*:ss* tt*", "T"
				};

				DateTime d = DateTime.ParseExact (str, time_formats, null,
				                                  DateTimeStyles.AllowWhiteSpaces);
				date = FMDateTime.ServerNow;
				date.Hour = d.Hour;
				date.Minute = d.Minute;
				date.Second = d.Second;
				date.Precision = FMDateTimePrecision.DateAndTime;
				return true;
			} catch { }
			*/

			return false;
		}

		private static bool ParseInternalFormat (string str, ref FMDateTime date)
		{
			FMDateTime orig_date = date;
		
			if (internal_format.IsMatch (str)) {
				Match m = internal_format.Match (str);
				if (m != null && m.Groups.Count == 8) {
					int year, month, day, hour, minute, second;

					int.TryParse(m.Groups[1].Value, out year);
					year += 1700;

					int.TryParse(m.Groups[2].Value, out month);
					int.TryParse(m.Groups[3].Value, out day);
					int.TryParse(m.Groups[5].Value, out hour);
                    int.TryParse(m.Groups[6].Value, out minute);
                    int.TryParse(m.Groups[7].Value, out second);

					// 1 digit hours apparently have just
					// had the trailing 0 dropped.  Go figure.
					if (m.Groups[5].Value.Length == 1) {
						hour *= 10;
					}

					// 1 digit minutes do too
					if (m.Groups[6].Value.Length == 1) {
						minute *= 10;
					}

					// 1 digit seconds aren't to be left out
					if (m.Groups[7].Value.Length == 1) {
						second *= 10;
					}

					if (!ValidateYear (year)) {
						return false;
					}

					if (date == null) {
						date = new FMDateTime ();
					}
					
					date.Year = year;

					date.Precision = FMDateTimePrecision.YearOnly;
					if (m.Groups[5].Value != String.Empty
					    && month > 0 && day > 0 && hour > 0) {
						if (!ValidateDate (year, month, day)
						    || !ValidateTime (hour, minute, second)) {
							date = orig_date;
							return false;
						}

						date.Month = month;
						date.Day = day;
						date.Hour = hour;
						date.Minute = minute;
						date.Second = second;

						date.Precision = FMDateTimePrecision.DateAndTime;
					} else if (m.Groups[3].Value != String.Empty
					           && month > 0 && day > 0) {
						if (!ValidateDate (year, month, day)) {
							date = orig_date;
							return false;
						}

						date.Month = month;
						date.Day = day;

						date.Precision = FMDateTimePrecision.DateOnly;
					} else if (m.Groups[2].Value != String.Empty
					           && month > 0) {
						if (!ValidateYearMonth (year, month)) {
							date = orig_date;
							return false;
						}

						date.Month = month;

						date.Precision = FMDateTimePrecision.YearMonthOnly;
					}

					return true;
				}
			}

			return false;
		}

		private static bool ValidateYear (int year)
		{
			// Sadly, we can't use MaxValue and MinValue due to
			// this function being used in the
			// parsing and initialization of those values
			if (year < 1700 || year > 2699) {
				return false;
			}
			
			return true;
		}

		private static bool ValidateYearMonth (int year, int month)
		{
			if (!ValidateYear (year)) {
				return false;
			}

			int num_months = CurrentCalendar.GetMonthsInYear (year);
			if (month < 1 || month > num_months) {
				return false;
			}
			
			return true;
		}

		private static bool ValidateDate (int year, int month, int day)
		{
			if (!ValidateYearMonth (year, month)) {
				return false;
			}

			int num_days = CurrentCalendar.GetDaysInMonth (year, month);
			if (day < 1 || day > num_days) {
				return false;
			}

			return true;
		}

		private static bool ValidateTime (int hour, int minute, int second)
		{
			if (hour < 0 || hour > 24) {
				return false;
			}

			if (minute < 0 || minute > 59) {
				return false;
			}

			if (second < 0 || second > 59) {
				return false;
			}

			if (hour == 24 && (minute > 0 || second > 0)) {
				return false;
			}

			return true;
		}
	}

	public enum FMDateTimePrecision {
		YearOnly,
		YearMonthOnly,
		DateOnly,
		DateAndTime
	}
}
