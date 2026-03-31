using System;
using System.Windows;
using System.Windows.Media;

namespace DepositCalculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка и получение входных данных
                if (!ValidateInputs(out decimal amount, out int months, out decimal annualRate))
                    return;

                // Расчет простых процентов
                decimal simpleInterest = CalculateSimpleInterest(amount, months, annualRate);

                // Расчет сложных процентов
                decimal compoundInterest = CalculateCompoundInterest(amount, months, annualRate);

                // Расчет разницы
                decimal difference = compoundInterest - simpleInterest;

                // Отображение результатов
                txtSimpleInterest.Text = $"{simpleInterest:N2} ₽ (Итоговая сумма: {(amount + simpleInterest):N2} ₽)";
                txtCompoundInterest.Text = $"{compoundInterest:N2} ₽ (Итоговая сумма: {(amount + compoundInterest):N2} ₽)";
                txtDifference.Text = $"{difference:N2} ₽";

                // Подсветка разницы
                if (difference > 0)
                {
                    txtDifference.Foreground = new SolidColorBrush(Colors.Green);
                }
                else if (difference < 0)
                {
                    txtDifference.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    txtDifference.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Проверка корректности введенных данных
        /// </summary>
        private bool ValidateInputs(out decimal amount, out int months, out decimal annualRate)
        {
            amount = 0;
            months = 0;
            annualRate = 0;

            // Проверка суммы вклада
            if (!decimal.TryParse(txtAmount.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму вклада (положительное число).",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAmount.Focus();
                return false;
            }

            // Проверка срока вклада
            if (!int.TryParse(txtMonths.Text, out months) || months <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректный срок вклада (положительное целое число месяцев).",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMonths.Focus();
                return false;
            }

            // Проверка процентной ставки
            if (!decimal.TryParse(txtRate.Text, out annualRate) || annualRate < 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную процентную ставку (неотрицательное число).",
                    "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtRate.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Расчет простых процентов
        /// Простые проценты начисляются на указанную сумму в конце срока вклада
        /// </summary>
        private decimal CalculateSimpleInterest(decimal amount, int months, decimal annualRate)
        {
            // Формула простых процентов: P * r * t / 12
            // где P - сумма вклада, r - годовая ставка (в долях), t - количество месяцев
            decimal monthlyRate = annualRate / 100 / 12;
            return amount * monthlyRate * months;
        }

        /// <summary>
        /// Расчет сложных процентов с ежемесячной капитализацией
        /// Сложные проценты начисляются ежемесячно, проценты за месяц прибавляются к сумме вклада
        /// </summary>
        private decimal CalculateCompoundInterest(decimal amount, int months, decimal annualRate)
        {
            if (months == 0)
                return 0;

            // Формула сложных процентов: A = P * (1 + r)^n - P
            // где P - сумма вклада, r - месячная ставка (в долях), n - количество месяцев
            decimal monthlyRate = annualRate / 100 / 12;
            decimal currentAmount = amount;

            // Альтернативный вариант с использованием цикла для наглядности
            for (int month = 1; month <= months; month++)
            {
                decimal monthlyInterest = currentAmount * monthlyRate;
                currentAmount += monthlyInterest;
            }

            return currentAmount - amount;
        }
    }
}