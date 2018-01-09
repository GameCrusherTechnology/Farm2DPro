using UnityEngine;
using System.Collections;
using System;
public class TimeManager 
{

	//time
	private static TimeSpan timeDelay = new TimeSpan(0) ;
	public static void setSystemTime(double newTime){

		DateTime dTime = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
		long ltime = long.Parse (newTime + "0000000");
		TimeSpan toNow = new TimeSpan (ltime);
		DateTime curTime = dTime.Add (toNow);

		timeDelay = curTime - System.DateTime.Now;
	}

	public static DateTime getSystemTime(){
		return  System.DateTime.Now - timeDelay;
	}
	public static int CurrentSystemNum{
		get{
			DateTime dTime = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
			TimeSpan s = System.DateTime.Now - dTime;
			return  (int)(System.DateTime.Now - dTime).TotalSeconds;
		}
	}

	public static string getTimeLeftString(int t){
		TimeSpan s = new TimeSpan (0,0,t);
		return s.ToString ();
	}
}

