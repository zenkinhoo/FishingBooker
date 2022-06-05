using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hooking.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Index => "Index";

        public static string Email => "Email";

        public static string ChangePassword => "ChangePassword";

        public static string DownloadPersonalData => "DownloadPersonalData";

        public static string DeletePersonalData => "DeletePersonalData";

        public static string ExternalLogins => "ExternalLogins";

        public static string PersonalData => "PersonalData";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        public static string MyCottages => "MyCottages";
        public static string MyBoats => "MyBoats";

        public static string MyCottageReservations => "MyCottageReservations";

        public static string CottageReservationsHistory => "CottageReservationsHistory";

        public static string MySpecialOffers => "MySpecialOffers";

        public static string BoatReservationHistory => "BoatReservationHistory";
        public static string MyBoatReservations => "MyBoatReservations";

        public static string CottagesReservationsHistory => "CottagesReservationHistory";
        public static string UserDeleteRequest => "UserDeleteRequest";
        public static string CottagesReservations => "CottagesReservations";
        public static string UserCottageFavorites => "UserCottageFavorites";
        public static string UserBoatFavorites => "UserBoatFavorites";

        public static string InstructorReservationsHistory => "InstructorReservationHistory";

        public static string AdventureReservationsHistory => "AdventureReservationsHistory";
        public static string AdventureReservations => "AdventureReservations";
        public static string UserAdventureFavorites => "UserAdventureFavorites";
        public static string BoatReservationsHistory => "BoatReservationsHistory";
        public static string BoatReservations => "BoatReservations";
        public static string BoatSpecialOffers => "BoatSpecialOffers";
        public static string CottageReports => "CottageReports";
        public static string CottageReportForm => "CottageReportForm";
        public static string BoatReports => "BoatReports";
        public static string BoatReportForm => "BoatReportForm";
        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DownloadPersonalData);

        public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        public static string MyCottagesNavClass(ViewContext viewContext) => PageNavClass(viewContext, MyCottages);

        public static string MyBoatsNavClass(ViewContext viewContext) => PageNavClass(viewContext, MyBoats);

        public static string MyCottageReservationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, MyCottageReservations);

        public static string CottageReservationsHistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, CottageReservationsHistory);

        public static string MySpecialOffersNavClass(ViewContext viewContext) => PageNavClass(viewContext, MySpecialOffers);

        public static string BoatReservatiosHistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, BoatReservationHistory);

        public static string MyBoatReservationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, MyBoatReservations);
        public static string CottagesReservationsHistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, CottagesReservationsHistory);
        public static string UserDeleteRequestNavClass(ViewContext viewContext) => PageNavClass(viewContext, UserDeleteRequest);
        public static string CottagesReservationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, CottagesReservations);

        public static string UserCottageFavoritesNavClass(ViewContext viewContext) => PageNavClass(viewContext, UserCottageFavorites);

        public static string UserBoatFavoritesNavClass(ViewContext viewContext) => PageNavClass(viewContext, UserBoatFavorites);
        public static string InstructorReservationsHistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, InstructorReservationsHistory);
        public static string AdventureReservationsHistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, AdventureReservationsHistory);

        public static string AdventureReservationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, AdventureReservations);
        public static string UserAdventureFavoritesNavClass(ViewContext viewContext) => PageNavClass(viewContext, UserAdventureFavorites);
        public static string BoatReservationsHistoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, BoatReservationsHistory);
        public static string BoatReservationsNavClass(ViewContext viewContext) => PageNavClass(viewContext, BoatReservations);
        public static string BoatSpecialOffersNavClass(ViewContext viewContext) => PageNavClass(viewContext, BoatSpecialOffers);
        public static string CottageReportsNavClass(ViewContext viewContext) => PageNavClass(viewContext, CottageReports);
        public static string CottageReportFormNavClass(ViewContext viewContext) => PageNavClass(viewContext, CottageReportForm);
        public static string BoatReportsNavClass(ViewContext viewContext) => PageNavClass(viewContext, BoatReports);
        public static string BoatReportFormNavClass(ViewContext viewContext) => PageNavClass(viewContext, BoatReportForm);
        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
