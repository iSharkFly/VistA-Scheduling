namespace IndianHealthService.ClinicalScheduling
{
    using System;
    /// <summary>
    /// Custom Event Args for SelectionChange. Don't know totally what it does yet.
    /// Changed to automatic properties.
    /// </summary>
    [Serializable]
    public class CGSelectionChangedArgs : EventArgs
    {
        public DateTime EndTime {get; set;}

        public string Resource {get; set;}

        public DateTime StartTime {get; set;}
    }
}

