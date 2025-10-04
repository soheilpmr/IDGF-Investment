using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDGF.Core.Domain
{

    public enum BondTypesEnum 
    {
        /// <summary>
        /// خزانه اسلامی
        /// </summary>
        IslamicTreasury=1,
        /// <summary>
        /// ochki l,\k
        /// </summary>
        CoponTreasury= 2,
        /// <summary>
        /// اوراق اجاره دولت
        /// </summary>
        GovernmentBond= 3,
        /// <summary>
        /// اوراق مرابحه
        /// </summary>
        MurabahaBond=4,
        /// <summary>
        /// اوراق مشارکت
        /// </summary>
        PartnershipBond=5,
    }
}
