#region Usings
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Neo.IronLua;
#endregion

// source: http://www.codeproject.com/Articles/19274/A-printf-implementation-in-C

namespace AT.MIN
{
	static class Tools
	{
		#region Public Methods
		#region IsNumericType
		/// <summary>
		/// Determines whether the specified value is of numeric type.
		/// </summary>
		/// <param name="o">The object to check.</param>
		/// <returns>
		/// 	<c>true</c> if o is a numeric type; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNumericType(object o)
		{
			return (o is byte ||
			  o is sbyte ||
			  o is short ||
			  o is ushort ||
			  o is int ||
			  o is uint ||
			  o is long ||
			  o is ulong ||
			  o is float ||
			  o is double ||
			  o is decimal);
		}
		#endregion
		#region IsPositive
		/// <summary>
		/// Determines whether the specified value is positive.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <param name="ZeroIsPositive">if set to <c>true</c> treats 0 as positive.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is positive; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPositive(object Value, bool ZeroIsPositive)
		{
			switch (LuaEmit.GetTypeCode(Value.GetType()))
			{
				case LuaEmitTypeCode.SByte:
					return (ZeroIsPositive ? (sbyte)Value >= 0 : (sbyte)Value > 0);
				case LuaEmitTypeCode.Int16:
					return (ZeroIsPositive ? (short)Value >= 0 : (short)Value > 0);
				case LuaEmitTypeCode.Int32:
					return (ZeroIsPositive ? (int)Value >= 0 : (int)Value > 0);
				case LuaEmitTypeCode.Int64:
					return (ZeroIsPositive ? (long)Value >= 0 : (long)Value > 0);
				case LuaEmitTypeCode.Single:
					return (ZeroIsPositive ? (float)Value >= 0 : (float)Value > 0);
				case LuaEmitTypeCode.Double:
					return (ZeroIsPositive ? (double)Value >= 0 : (double)Value > 0);
				case LuaEmitTypeCode.Decimal:
					return (ZeroIsPositive ? (decimal)Value >= 0 : (decimal)Value > 0);
				case LuaEmitTypeCode.Byte:
					return (ZeroIsPositive ? true : (byte)Value > 0);
				case LuaEmitTypeCode.UInt16:
					return (ZeroIsPositive ? true : (ushort)Value > 0);
				case LuaEmitTypeCode.UInt32:
					return (ZeroIsPositive ? true : (uint)Value > 0);
				case LuaEmitTypeCode.UInt64:
					return (ZeroIsPositive ? true : (ulong)Value > 0);
				case LuaEmitTypeCode.Char:
					return (ZeroIsPositive ? true : (char)Value != '\0');
				default:
					return false;
			}
		}
		#endregion
		#region ToUnsigned
		/// <summary>
		/// Converts the specified values boxed type to its correpsonding unsigned
		/// type.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns>A boxed numeric object whos type is unsigned.</returns>
		public static object ToUnsigned(object Value)
		{
			switch (LuaEmit.GetTypeCode(Value.GetType()))
			{
				case LuaEmitTypeCode.SByte:
					return (byte)((sbyte)Value);
				case LuaEmitTypeCode.Int16:
					return (ushort)((short)Value);
				case LuaEmitTypeCode.Int32:
					return (uint)((int)Value);
				case LuaEmitTypeCode.Int64:
					return (ulong)((long)Value);

				case LuaEmitTypeCode.Byte:
					return Value;
				case LuaEmitTypeCode.UInt16:
					return Value;
				case LuaEmitTypeCode.UInt32:
					return Value;
				case LuaEmitTypeCode.UInt64:
					return Value;

				case LuaEmitTypeCode.Single:
					return (UInt32)((float)Value);
				case LuaEmitTypeCode.Double:
					return (ulong)((double)Value);
				case LuaEmitTypeCode.Decimal:
					return (ulong)((decimal)Value);

				default:
					return null;
			}
		}
		#endregion
		#region ToInteger
		/// <summary>
		/// Converts the specified values boxed type to its correpsonding integer
		/// type.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <param name="Round"></param>
		/// <returns>A boxed numeric object whos type is an integer type.</returns>
		public static object ToInteger(object Value, bool Round)
		{
			switch (LuaEmit.GetTypeCode(Value.GetType()))
			{
				case LuaEmitTypeCode.SByte:
					return Value;
				case LuaEmitTypeCode.Int16:
					return Value;
				case LuaEmitTypeCode.Int32:
					return Value;
				case LuaEmitTypeCode.Int64:
					return Value;

				case LuaEmitTypeCode.Byte:
					return Value;
				case LuaEmitTypeCode.UInt16:
					return Value;
				case LuaEmitTypeCode.UInt32:
					return Value;
				case LuaEmitTypeCode.UInt64:
					return Value;

				case LuaEmitTypeCode.Single:
					return (Round ? (int)Math.Round((float)Value) : (int)((float)Value));
				case LuaEmitTypeCode.Double:
					return (Round ? (long)Math.Round((double)Value) : (long)((double)Value));
				case LuaEmitTypeCode.Decimal:
					return (Round ? Math.Round((decimal)Value) : (decimal)Value);

				default:
					return null;
			}
		}
		#endregion
		#region UnboxToLong
		public static long UnboxToLong(object Value, bool Round)
		{
			switch (LuaEmit.GetTypeCode(Value.GetType()))
			{
				case LuaEmitTypeCode.SByte:
					return (long)((sbyte)Value);
				case LuaEmitTypeCode.Int16:
					return (long)((short)Value);
				case LuaEmitTypeCode.Int32:
					return (long)((int)Value);
				case LuaEmitTypeCode.Int64:
					return (long)Value;

				case LuaEmitTypeCode.Byte:
					return (long)((byte)Value);
				case LuaEmitTypeCode.UInt16:
					return (long)((ushort)Value);
				case LuaEmitTypeCode.UInt32:
					return (long)((uint)Value);
				case LuaEmitTypeCode.UInt64:
					return (long)((ulong)Value);

				case LuaEmitTypeCode.Single:
					return (Round ? (long)Math.Round((float)Value) : (long)((float)Value));
				case LuaEmitTypeCode.Double:
					return (Round ? (long)Math.Round((double)Value) : (long)((double)Value));
				case LuaEmitTypeCode.Decimal:
					return (Round ? (long)Math.Round((decimal)Value) : (long)((decimal)Value));

				default:
					return 0;
			}
		}
		#endregion
		#region ReplaceMetaChars
		/// <summary>
		/// Replaces the string representations of meta chars with their corresponding
		/// character values.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>A string with all string meta chars are replaced</returns>
		public static string ReplaceMetaChars(string input)
		{
			return Regex.Replace(input, @"(\\)(\d{3}|[^\d])?", new MatchEvaluator(ReplaceMetaCharsMatch));
		}
		private static string ReplaceMetaCharsMatch(Match m)
		{
			// convert octal quotes (like \040)
			if (m.Groups[2].Length == 3)
				return Convert.ToChar(Convert.ToByte(m.Groups[2].Value, 8)).ToString();
			else
			{
				// convert all other special meta characters
				//TODO: \xhhh hex and possible dec !!
				switch (m.Groups[2].Value)
				{
					case "0":           // null
						return "\0";
					case "a":           // alert (beep)
						return "\a";
					case "b":           // BS
						return "\b";
					case "f":           // FF
						return "\f";
					case "v":           // vertical tab
						return "\v";
					case "r":           // CR
						return "\r";
					case "n":           // LF
						return "\n";
					case "t":           // Tab
						return "\t";
					default:
						// if neither an octal quote nor a special meta character
						// so just remove the backslash
						return m.Groups[2].Value;
				}
			}
		}
		#endregion
		#region fprintf
		public static void fprintf(TextWriter Destination, string Format, params object[] Parameters)
		{
			Destination.Write(Tools.sprintf(Format, Parameters));
		}
		#endregion
		#region sprintf
		public static string sprintf(string Format, params object[] Parameters)
		{
			#region Variables
			StringBuilder f = new StringBuilder();
			Regex r = new Regex(@"\%(\d*\$)?([\'\#\-\+ ]*)(\d*)(?:\.(\d+))?([hl])?([dioxXucsfeEgGpnq%])");
			//"%[parameter][flags][width][.precision][length]type"
			Match m = null;
			string w = String.Empty;
			int defaultParamIx = 0;
			int paramIx;
			object o = null;

			bool flagLeft2Right = false;
			bool flagAlternate = false;
			bool flagPositiveSign = false;
			bool flagPositiveSpace = false;
			bool flagZeroPadding = false;
			bool flagGroupThousands = false;

			int fieldLength = 0;
			int fieldPrecision = 0;
			char shortLongIndicator = '\0';
			char formatSpecifier = '\0';
			char paddingCharacter = ' ';
			#endregion

			// find all format parameters in format string
			f.Append(Format);
			m = r.Match(f.ToString());
			while (m.Success)
			{
				#region parameter index
				paramIx = defaultParamIx;
				if (m.Groups[1] != null && m.Groups[1].Value.Length > 0)
				{
					string val = m.Groups[1].Value.Substring(0, m.Groups[1].Value.Length - 1);
					paramIx = Convert.ToInt32(val) - 1;
				};
				#endregion

				#region format flags
				// extract format flags
				flagAlternate = false;
				flagLeft2Right = false;
				flagPositiveSign = false;
				flagPositiveSpace = false;
				flagZeroPadding = false;
				flagGroupThousands = false;
				if (m.Groups[2] != null && m.Groups[2].Value.Length > 0)
				{
					string flags = m.Groups[2].Value;

					flagAlternate = (flags.IndexOf('#') >= 0);
					flagLeft2Right = (flags.IndexOf('-') >= 0);
					flagPositiveSign = (flags.IndexOf('+') >= 0);
					flagPositiveSpace = (flags.IndexOf(' ') >= 0);
					flagGroupThousands = (flags.IndexOf('\'') >= 0);

					// positive + indicator overrides a
					// positive space character
					if (flagPositiveSign && flagPositiveSpace)
						flagPositiveSpace = false;
				}
				#endregion

				#region field length
				// extract field length and 
				// pading character
				paddingCharacter = ' ';
				fieldLength = int.MinValue;
				if (m.Groups[3] != null && m.Groups[3].Value.Length > 0)
				{
					fieldLength = Convert.ToInt32(m.Groups[3].Value);
					flagZeroPadding = (m.Groups[3].Value[0] == '0');
				}
				#endregion

				if (flagZeroPadding)
					paddingCharacter = '0';

				// left2right allignment overrides zero padding
				if (flagLeft2Right && flagZeroPadding)
				{
					flagZeroPadding = false;
					paddingCharacter = ' ';
				}

				#region field precision
				// extract field precision
				fieldPrecision = int.MinValue;
				if (m.Groups[4] != null && m.Groups[4].Value.Length > 0)
					fieldPrecision = Convert.ToInt32(m.Groups[4].Value);
				#endregion

				#region short / long indicator
				// extract short / long indicator
				shortLongIndicator = Char.MinValue;
				if (m.Groups[5] != null && m.Groups[5].Value.Length > 0)
					shortLongIndicator = m.Groups[5].Value[0];
				#endregion

				#region format specifier
				// extract format
				formatSpecifier = Char.MinValue;
				if (m.Groups[6] != null && m.Groups[6].Value.Length > 0)
					formatSpecifier = m.Groups[6].Value[0];
				#endregion

				// default precision is 6 digits if none is specified except
				if (fieldPrecision == int.MinValue &&
				  formatSpecifier != 's' &&
				  formatSpecifier != 'c' &&
				  Char.ToUpper(formatSpecifier) != 'X' &&
				  formatSpecifier != 'o')
					fieldPrecision = 6;

				#region get next value parameter
				// get next value parameter and convert value parameter depending on short / long indicator
				if (Parameters == null || paramIx >= Parameters.Length)
					o = null;
				else
				{
					o = Parameters[paramIx];

					if (shortLongIndicator == 'h')
					{
						if (o is int)
							o = (short)((int)o);
						else if (o is long)
							o = (short)((long)o);
						else if (o is uint)
							o = (ushort)((uint)o);
						else if (o is ulong)
							o = (ushort)((ulong)o);
					}
					else if (shortLongIndicator == 'l')
					{
						if (o is short)
							o = (long)((short)o);
						else if (o is int)
							o = (long)((int)o);
						else if (o is ushort)
							o = (ulong)((ushort)o);
						else if (o is uint)
							o = (ulong)((uint)o);
					}
				}
				#endregion

				// convert value parameters to a string depending on the formatSpecifier
				w = String.Empty;
				switch (formatSpecifier)
				{
					#region % - character
					case '%':   // % character
						w = "%";
						break;
					#endregion
					#region d - integer
					case 'd':   // integer
						w = FormatNumber((flagGroupThousands ? "n" : "d"), flagAlternate,
								fieldLength, int.MinValue, flagLeft2Right,
								flagPositiveSign, flagPositiveSpace,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region i - integer
					case 'i':   // integer
						goto case 'd';
					#endregion
					#region o - octal integer
					case 'o':   // octal integer - no leading zero
						w = FormatOct("o", flagAlternate,
								fieldLength, int.MinValue, flagLeft2Right,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region x - hex integer
					case 'x':   // hex integer - no leading zero
						w = FormatHex("x", flagAlternate,
								fieldLength, fieldPrecision, flagLeft2Right,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region X - hex integer
					case 'X':   // same as x but with capital hex characters
						w = FormatHex("X", flagAlternate,
								fieldLength, fieldPrecision, flagLeft2Right,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region u - unsigned integer
					case 'u':   // unsigned integer
						w = FormatNumber((flagGroupThousands ? "n" : "d"), flagAlternate,
								fieldLength, int.MinValue, flagLeft2Right,
								false, false,
								paddingCharacter, ToUnsigned(o));
						defaultParamIx++;
						break;
					#endregion
					#region c - character
					case 'c':   // character
						if (IsNumericType(o))
							w = Convert.ToChar(o).ToString();
						else if (o is char)
							w = ((char)o).ToString();
						else if (o is string && ((string)o).Length > 0)
							w = ((string)o)[0].ToString();
						defaultParamIx++;
						break;
					#endregion
					#region s - string
					case 's':   // string
						//string t = "{0" + (fieldLength != int.MinValue ? "," + (flagLeft2Right ? "-" : String.Empty) + fieldLength.ToString() : String.Empty) + ":s}";
						w = o.ToString();
						if (fieldPrecision >= 0)
							w = w.Substring(0, fieldPrecision);

						if (fieldLength != int.MinValue)
							if (flagLeft2Right)
								w = w.PadRight(fieldLength, paddingCharacter);
							else
								w = w.PadLeft(fieldLength, paddingCharacter);
						defaultParamIx++;
						break;
					#endregion
					#region -- q - string --
					case 'q':
						using (var tw = new StringWriter())
						{
							Lua.RtWriteValue(tw, o, false, 0, String.Empty);
							w = tw.GetStringBuilder().ToString();
						}
						break;
					#endregion
					#region f - double number
					case 'f':   // double
						w = FormatNumber((flagGroupThousands ? "n" : "f"), flagAlternate,
								fieldLength, fieldPrecision, flagLeft2Right,
								flagPositiveSign, flagPositiveSpace,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region e - exponent number
					case 'e':   // double / exponent
						w = FormatNumber("e", flagAlternate,
								fieldLength, fieldPrecision, flagLeft2Right,
								flagPositiveSign, flagPositiveSpace,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region E - exponent number
					case 'E':   // double / exponent
						w = FormatNumber("E", flagAlternate,
								fieldLength, fieldPrecision, flagLeft2Right,
								flagPositiveSign, flagPositiveSpace,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region g - general number
					case 'g':   // double / exponent
						w = FormatNumber("g", flagAlternate,
								fieldLength, fieldPrecision, flagLeft2Right,
								flagPositiveSign, flagPositiveSpace,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region G - general number
					case 'G':   // double / exponent
						w = FormatNumber("G", flagAlternate,
								fieldLength, fieldPrecision, flagLeft2Right,
								flagPositiveSign, flagPositiveSpace,
								paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region p - pointer
					case 'p':   // pointer
						if (o is IntPtr)
							w = "0x" + ((IntPtr)o).ToString("x");
						defaultParamIx++;
						break;
					#endregion
					#region n - number of processed chars so far
					case 'n':   // number of characters so far
						w = FormatNumber("d", flagAlternate,
								fieldLength, int.MinValue, flagLeft2Right,
								flagPositiveSign, flagPositiveSpace,
								paddingCharacter, m.Index);
						break;
					#endregion
					default:
						w = String.Empty;
						defaultParamIx++;
						break;
				}

				// replace format parameter with parameter value
				// and start searching for the next format parameter
				// AFTER the position of the current inserted value
				// to prohibit recursive matches if the value also
				// includes a format specifier
				f.Remove(m.Index, m.Length);
				f.Insert(m.Index, w);
				m = r.Match(f.ToString(), m.Index + w.Length);
			}

			return f.ToString();
		}
		#endregion
		#endregion

		#region Private Methods
		#region FormatOCT
		private static string FormatOct(string NativeFormat, bool Alternate,
						  int FieldLength, int FieldPrecision,
						  bool Left2Right,
						  char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
							"," + (Left2Right ?
								"-" :
								String.Empty) + FieldLength.ToString() :
							String.Empty) + "}";

			if (IsNumericType(Value))
			{
				w = Convert.ToString(UnboxToLong(Value, true), 8);

				if (Left2Right || Padding == ' ')
				{
					if (Alternate && w != "0")
						w = "0" + w;
					w = String.Format(lengthFormat, w);
				}
				else
				{
					if (FieldLength != int.MinValue)
						w = w.PadLeft(FieldLength - (Alternate && w != "0" ? 1 : 0), Padding);
					if (Alternate && w != "0")
						w = "0" + w;
				}
			}

			return w;
		}
		#endregion
		#region FormatHEX
		private static string FormatHex(string NativeFormat, bool Alternate,
						  int FieldLength, int FieldPrecision,
						  bool Left2Right,
						  char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
							"," + (Left2Right ?
								"-" :
								String.Empty) + FieldLength.ToString() :
							String.Empty) + "}";
			string numberFormat = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ?
							FieldPrecision.ToString() :
							String.Empty) + "}";

			if (IsNumericType(Value))
			{
				w = String.Format(numberFormat, Value);

				if (Left2Right || Padding == ' ')
				{
					if (Alternate)
						w = (NativeFormat == "x" ? "0x" : "0X") + w;
					w = String.Format(lengthFormat, w);
				}
				else
				{
					if (FieldLength != int.MinValue)
						w = w.PadLeft(FieldLength - (Alternate ? 2 : 0), Padding);
					if (Alternate)
						w = (NativeFormat == "x" ? "0x" : "0X") + w;
				}
			}

			return w;
		}
		#endregion
		#region FormatNumber
		private static string FormatNumber(string NativeFormat, bool Alternate,
						  int FieldLength, int FieldPrecision,
						  bool Left2Right,
						  bool PositiveSign, bool PositiveSpace,
						  char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
							"," + (Left2Right ?
								"-" :
								String.Empty) + FieldLength.ToString() :
							String.Empty) + "}";
			string numberFormat = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ?
							FieldPrecision.ToString() :
							"0") + "}";

			if (IsNumericType(Value))
			{
				w = String.Format(numberFormat, Value);

				if (Left2Right || Padding == ' ')
				{
					if (IsPositive(Value, true))
						w = (PositiveSign ?
							"+" : (PositiveSpace ? " " : String.Empty)) + w;
					w = String.Format(lengthFormat, w);
				}
				else
				{
					if (w.StartsWith("-"))
						w = w.Substring(1);
					if (FieldLength != int.MinValue)
						w = w.PadLeft(FieldLength, Padding);
					if (IsPositive(Value, true))
						w = (PositiveSign ?
							"+" : (PositiveSpace ?
								" " : String.Empty)) + w;
					else
						w = "-" + w;
				}
			}

			return w;
		}
		#endregion
		#endregion

		public delegate string DateTimeDelegate(DateTime dateTime);

		private static readonly Dictionary<string, DateTimeDelegate> Formats = new Dictionary<string, DateTimeDelegate>
	{
      // abbreviated weekday name (e.g., Wed)
      { "a", (dateTime) => dateTime.ToString("ddd", CultureInfo.CurrentCulture) },
      // full weekday name (e.g., Wednesday)
      { "A", (dateTime) => dateTime.ToString("dddd", CultureInfo.CurrentCulture) },
      // abbreviated month name (e.g., Sep)
      { "b", (dateTime) => dateTime.ToString("MMM", CultureInfo.CurrentCulture) },
      // full month name (e.g., September)
      { "B", (dateTime) => dateTime.ToString("MMMM", CultureInfo.CurrentCulture) },
      // date and time (e.g., 09/16/98 23:48:10)
      { "c", (dateTime) => dateTime.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.CurrentCulture) },
      // day of the month (16) (01-31)
      { "d", (dateTime) => dateTime.ToString("dd", CultureInfo.CurrentCulture) },
      // day of the month, space-padded ( 1-31)
      { "e", (dateTime) => dateTime.ToString("%d", CultureInfo.CurrentCulture).PadLeft(2, ' ') },
      // hour, using a 24-hour clock (00-23)
      { "H", (dateTime) => dateTime.ToString("HH", CultureInfo.CurrentCulture) },
      // hour, using a 12-hour clock (01-12)
      { "I", (dateTime) => dateTime.ToString("hh", CultureInfo.CurrentCulture) },
      // day of the year (001-366)
      { "j", (dateTime) => dateTime.DayOfYear.ToString().PadLeft(3, '0') },
      //month (01-12)
      { "m", (dateTime) => dateTime.ToString("MM", CultureInfo.CurrentCulture) },
      // minute (00-59)
      { "M", (dateTime) => dateTime.Minute.ToString().PadLeft(2, '0') },
      // either "AM" or "PM"
      { "p", (dateTime) => dateTime.ToString("tt",new CultureInfo("en-US")) },
      // second (00-59)
      { "S", (dateTime) => dateTime.ToString("ss", CultureInfo.CurrentCulture) },
      // week number with the first Sunday as the first day of week one (00-53)   
      { "U", (dateTime) => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Sunday).ToString().PadLeft(2, '0') },
      // week number with the first Monday as the first day of week one (00-53)
      { "W", (dateTime) => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday).ToString().PadLeft(2, '0') },
      // weekday as a decimal number with Sunday as 0 (0-6)
      { "w", (dateTime) => ((int) dateTime.DayOfWeek).ToString() },
      // date (e.g., 09/16/98)
      { "x", (dateTime) => dateTime.ToString("d", CultureInfo.CurrentCulture) },
      // time (e.g., 23:48:10)
      { "X", (dateTime) => dateTime.ToString("T", CultureInfo.CurrentCulture) },
      // two-digit year [00-99]
      { "y", (dateTime) => dateTime.ToString("yy", CultureInfo.CurrentCulture) },
      // full year (e.g., 2014)
      { "Y", (dateTime) => dateTime.ToString("yyyy", CultureInfo.CurrentCulture) },
      // Timezone name or abbreviation, If timezone cannot be termined, no characters
      { "Z", (dateTime) => dateTime.ToString("zzz", CultureInfo.CurrentCulture) },
      // the character `%�
      { "%", (dateTime) => "%" }
	};
		// http://www.cplusplus.com/reference/ctime/strftime/

		/// <summary>
		/// Format time as string
		/// </summary>
		/// <param name="dateTime">Instant in time, typically expressed as a date and time of day.</param>
		/// <param name="pattern">String containing any combination of regular characters and special format specifiers.They all begin with a percentage (%).</param>
		/// <returns>String with expanding its format specifiers into the corresponding values that represent the time described in dateTime</returns>
		public static string ToStrFTime(this DateTime dateTime, string pattern)
		{
			string output = "";
			int n = 0;

			if (string.IsNullOrEmpty(pattern)) { return dateTime.ToString(); }

			while (n < pattern.Length)
			{
				string s = pattern.Substring(n, 1);

				if (n + 1 >= pattern.Length)
					output += s;
				else
					output += s == "%"
						? Formats.ContainsKey(pattern.Substring(++n, 1)) ? Formats[pattern.Substring(n, 1)].Invoke(dateTime) : "%" + pattern.Substring(n, 1)
						: s;
				n++;
			}

			return output;
		}
	}
}