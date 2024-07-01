using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace slon
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BirthYearTextBox.MaxLength = 4;
            LicenseYearTextBox.MaxLength = 4;
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(BirthYearTextBox.Text) ||
                string.IsNullOrWhiteSpace(LicenseYearTextBox.Text) ||
                LocationComboBox.SelectedIndex == -1 ||
                VehicleTypeComboBox.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(EnginePowerTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                int birthYear = int.Parse(BirthYearTextBox.Text);
                int licenseYear = int.Parse(LicenseYearTextBox.Text);

                // Проверка корректности года рождения и получения прав
                if (birthYear < 1920)
                {
                    MessageBox.Show("Год рождения должен быть не раньше 1920 года.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (licenseYear < birthYear)
                {
                    MessageBox.Show("Год получения прав должен быть больше или равен году рождения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string location = (LocationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                string vehicleType = (VehicleTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                int enginePower = int.Parse(EnginePowerTextBox.Text);

                double territoryCoeff = GetTerritoryCoeff(location);
                double ageAndExpCoeff = GetAgeAndExpCoeff(birthYear, licenseYear);
                double bonusCoeff = GetBonusCoeff(licenseYear);
                double powerCoeff = GetPowerCoeff(enginePower);

                double baseRate = GetBaseRate(vehicleType);
                double totalPrice = baseRate * territoryCoeff * ageAndExpCoeff * bonusCoeff * powerCoeff;

                // Проверка, чтобы стоимость ОСАГО не была меньше нуля
                if (totalPrice < 0)
                {
                    totalPrice = 0;
                }

                ResultTextBox.Text = totalPrice.ToString("C");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double GetTerritoryCoeff(string location)
        {
            switch (location)
            {
                case "Кузнецк":
                    return 1.0;
                case "Пенза":
                    return 1.4;
                case "Прочие города (Пензенская область)":
                    return 0.7;
                case "Сызрань":
                    return 1.1;
                case "Самара":
                    return 1.6;
                case "Тольятти":
                    return 1.5;
                case "Прочие города (Самарская область)":
                    return 0.9;
                case "Димитровград":
                    return 1.1;
                case "Ульяновск": return 1.4;
                case "Прочие города (Ульяновская область)":
                    return 0.8;
                default:
                    return 1.0;
            }
        }

        private double GetAgeAndExpCoeff(int birthYear, int licenseYear)
        {
            int age = DateTime.Now.Year - birthYear;
            int experience = DateTime.Now.Year - licenseYear;

            if (age <= 22 && experience <= 3)
                return 1.8;
            else if (age <= 22 && experience > 3)
                return 1.6;
            else if (age > 22 && experience <= 3)
                return 1.7;
            else
                return 1.0;
        }

        private double GetBonusCoeff(int licenseYear)
        {
            int experience = DateTime.Now.Year - licenseYear;
            return 3.92 - (experience * 0.173);
        }

        private double GetPowerCoeff(int enginePower)
        {
            if (enginePower <= 50)
                return 0.6;
            else if (enginePower <= 70)
                return 1.0;
            else if (enginePower <= 100)
                return 1.1;
            else if (enginePower <= 120)
                return 1.2;
            else if (enginePower <= 150)
                return 1.4;
            else
                return 1.6;
        }

        private double GetBaseRate(string vehicleType)
        {
            switch (vehicleType)
            {
                case "Мотоциклы, мотороллеры (категории А)":
                    return 1215;
                case "Легковые автомобили (категории В)":
                    return 1980;
                case "Грузовые автомобили (категории С)":
                    return 2025;
                default:
                    return 0;
            }
        }
    }
}