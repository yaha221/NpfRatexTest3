using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Data.SqlClient;
using NpfRatexTest3.Models;
using NpfRatexTest3.Commands;

namespace NpfRatexTest3.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region Поля

        private readonly SqlConnection _connection;
        private readonly SqlCommand _command;
        private SqlDependency _dependency;
        private string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Database=TestData;Integrated Security=True;Connect Timeout=30;TrustServerCertificate=False";
        private const string SelectQuery = "SELECT Id, Flag, DateNow FROM TestDatas";
        private SqlDataAdapter _dpAdapter;
        private List<TestData> TestDatas, oldTestDatas;
        private Thread DataThread;
        private DataTable _testdatas;
        #endregion

        public MainWindowViewModel()
        {
            if (NewConnectionString != null)
            {
                ConnectionString = NewConnectionString;
            }
            _connection = new SqlConnection(ConnectionString);
            _command = new SqlCommand(SelectQuery, _connection);
            OpenConnectionThreadCommand = new LambdaCommand(OnOpenConnectionThreadCommandExecute, CanOpenConnectionThreadCommandExecuted);
            CloseConnectionThreadCommand = new LambdaCommand(OnCloseConnectionThreadCommandExecute, CanCloseConnectionThreadCommandExecuted);
        }

        #region Команды

        #region Команда открытия подключения к бд в другом потоке

        public ICommand OpenConnectionThreadCommand { get; }

        private static bool CanOpenConnectionThreadCommandExecuted(object p) => true;

        private void OnOpenConnectionThreadCommandExecute(object p)
        {
            DataThread = new Thread(new ThreadStart(StartTestDatasMonitoring));
            DataThread.Start();
        }

        #endregion

        #region Команда закрытия подключения к бд в другом потоке

        public ICommand CloseConnectionThreadCommand { get; }

        private static bool CanCloseConnectionThreadCommandExecuted(object p) => true;

        private void OnCloseConnectionThreadCommandExecute(object p)
        {
            try
            {
                SqlDependency.Stop(ConnectionString);
                DataThread.Abort();
            }
            catch (Exception e)
            {
                
            }
        }

        #endregion


        #endregion

        #region Свойства

        #region Свойство для пердачи данных в DataGrid

        private List<TestData> _testDataGrid;

        public List<TestData> TestDataGrid
        {
            get => _testDataGrid;
            set => Set(ref _testDataGrid, value);
        }

        #endregion

        private string _newConnectionString;

        public string NewConnectionString { get; set; }

        #endregion

        #region Методы


        #region Начало отслеживания изменений в бд

        private void StartTestDatasMonitoring()
        {
            try
            {
                SqlDependency.Stop(ConnectionString);
                SqlDependency.Start(ConnectionString);

                RegisterSqlDependency();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion

        #region Регистрация SqlDependency

        private void RegisterSqlDependency()
        {

            _command.Notification = null;
            _dependency = new SqlDependency(_command);
            _dependency.OnChange += _dependency_OnChange;

            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            _command.ExecuteNonQuery();
            _connection.Close();
        }

        #endregion

        #region Получение данных из бд

        private void BindTestDatas()
        {
            try
            {
                SqlConnection connection = new SqlConnection(ConnectionString);
                _dpAdapter = new SqlDataAdapter(SelectQuery, connection);
                _testdatas = new DataTable();
                int i = _dpAdapter.Fill(_testdatas);
                oldTestDatas = TestDatas;
                TestDatas = (from DataRow dr in _testdatas.Rows
                    select new TestData()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Flag = Convert.ToBoolean(dr["Flag"]),
                        DateNow = Convert.ToString(dr["DateNow"])
                    }).ToList();
                if (oldTestDatas != null)
                {
                    bool IsEqual = oldTestDatas.SequenceEqual(TestDatas);
                    if (!IsEqual)
                        SendData();
                }
                else
                {
                    SendData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        #endregion

        #region Отправка данных в GUI

        private void SendData()
        {
            if (Dispatcher.CurrentDispatcher.CheckAccess())
            {

                Dispatcher.CurrentDispatcher.Invoke(delegate ()
                {
                     TestDataGrid = TestDatas;
                });
            }
            else
            {
                TestDataGrid = TestDatas;
            }
        }

        #endregion

        #endregion

        #region События

        private void _dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            var dependency = (SqlDependency)sender;
            dependency.OnChange -= _dependency_OnChange;
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.DataBind, new Action(BindTestDatas));
            RegisterSqlDependency();
        }

        #endregion
    }
}
