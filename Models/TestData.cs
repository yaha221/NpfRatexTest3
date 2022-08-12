using System;

namespace NpfRatexTest3.Models
{
    public class TestData : IEquatable<TestData>
    {
        public int Id { get; set; }

        #region Поле флага

        private bool _flag;
        
        public bool Flag
        {
            get => _flag;
            set => _flag = value;
        }

        #endregion

        #region Поле даты

        private string _dateNow;
        
        public string DateNow
        {
            get => _dateNow;
            set => _dateNow = value;
        }

        #endregion

        #region Сравнение объектов

        public bool Equals(TestData other)
        {
            if (other is null)
                return false;
            return this.Id == other.Id && this.Flag == other.Flag && this.DateNow == other.DateNow;
        }

        public override bool Equals(object? obj) => Equals(obj as TestData);

        public override int GetHashCode() => Id.GetHashCode()^Flag.GetHashCode()^DateNow.GetHashCode();

        #endregion
    }
}

