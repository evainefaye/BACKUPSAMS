//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class annoucement
    {
        public System.Guid announcementId { get; set; }
        public string enteredDate { get; set; }
        public string enteredBy { get; set; }
        public string announcementText { get; set; }
        public string seenBy { get; set; }
    
        public virtual user user { get; set; }
    }
}
