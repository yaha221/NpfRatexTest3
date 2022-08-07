using System;
using NpfRatexTest3.ViewModels;

namespace NpfRatexTest3.Models
{
    public class TestData:ViewModel
    {
        public int Id { get; set; }

        #region Поле флага

        private bool _flag;
        /// <summary> Поле флага</summary>
        public bool Flag
        {
            get => _flag;
            set => Set(ref _flag, value);
        }

        #endregion

        #region Поле даты

        private string _date;
        /// <summary> Поле даты </summary>
        public string Date
        {
            get => _date;
            set => Set(ref _date, value);
        }

        #endregion

    }
}
