using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSU_Library
{
	public interface IPowerSupply
    {
		/// <summary>
		/// 
		/// </summary>
		bool IsInitialized { get; }
		/// <summary>
		/// Turns power on/off the power supply output
		/// </summary>
		/// <param name="isOn">if true then turns on the power on the connected load</param>
		/// <returns>returns success</returns>
		public bool PowerSwitch(bool isOn);
		/// <summary>
		/// Sets PSU current
		/// </summary>
		/// <param name="value">Set current value in Amperes</param>
		/// <returns>Returns the set current</returns>
		public double? SetCurrent(double value);
		/// <summary>
		/// Sets the preset voltage value
		/// </summary>
		/// <param name="value">Set voltage value in Volts</param>
		/// <returns>Returns the set voltage</returns>
		double? SetVoltage(double value);
		/// <summary>
		/// Sets the set current and voltage values
		/// </summary>
		/// <param name="currency">Set current value in Ameras</param>
		/// <param name="voltage">Set voltage value in Volts</param>
		void SetPower(double currency, double voltage);
	}
}
