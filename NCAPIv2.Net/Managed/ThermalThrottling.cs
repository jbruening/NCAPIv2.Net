namespace NCAPIv2.Managed
{
    /// <summary>
    /// Throttling level due to heat level
    /// </summary>
    public enum ThermalThrottling : int
    {
        /// <summary>
        /// No limit reached.
        /// </summary>
        NoLimit = 0,
        /// <summary>
        /// Lower guard temperature threshold of chip sensor reached; short throttling time is in action between inferences to protect the device.
        /// </summary>
        LowerGuard = 1,
        /// <summary>
        /// Upper guard temperature of chip sensor reached; long throttling time is in action between inferences to protect the device.
        /// </summary>
        UpperGuard = 2
    }
}
