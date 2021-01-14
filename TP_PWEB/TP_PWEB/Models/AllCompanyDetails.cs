using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class AllCompanyDetails
    {
        public IEnumerable<Employee> allEmployes;
        public IEnumerable<AdminBusiness> allAdmBusinesses;
        public int num_pendingBookings;
        public int num_approvedBookings;
        public int num_checkedInBookings;
        public int num_checkedOutBookings;
        public int num_vehicles;
    }
}