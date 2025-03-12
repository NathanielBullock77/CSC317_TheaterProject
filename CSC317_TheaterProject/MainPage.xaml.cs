using System.Collections.Specialized;
using System.Formats.Asn1;
using System.Reflection.Metadata;
using System.Threading.Tasks;
#if IOS || MACCATALYST
using Foundation;
#endif

namespace CSC317_TheaterProject
{
    public class SeatingUnit
    {
        public string Name { get; set; }
        public bool Reserved { get; set; }

        public SeatingUnit(string name, bool reserved = false)
        {
            Name = name;
            Reserved = reserved;
        }

    }

    public partial class MainPage : ContentPage
    {
        SeatingUnit[,] seatingChart = new SeatingUnit[5, 10];

        public MainPage()
        {
            InitializeComponent();
            GenerateSeatingNames();
            RefreshSeating();
        }

        private async void ButtonReserveSeat(object sender, EventArgs e)
        {
            var seat = await DisplayPromptAsync("Enter Seat Number", "Enter seat number: ");

            if (seat != null)
            {
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == seat)
                        {
                            seatingChart[i, j].Reserved = true;
                            await DisplayAlert("Successfully Reserverd", "Your seat was reserverd successfully!", "Ok");
                            RefreshSeating();
                            return;
                        }
                    }
                }

                await DisplayAlert("Error", "Seat was not found.", "Ok");
            }
        }

        private void GenerateSeatingNames()
        {
            List<string> letters = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                letters.Add(c.ToString());
            }

            int letterIndex = 0;

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    seatingChart[row, column] = new SeatingUnit(letters[letterIndex] + (column + 1).ToString());
                }

                letterIndex++;
            }
        }

        private void RefreshSeating()
        {
            grdSeatingView.RowDefinitions.Clear();
            grdSeatingView.ColumnDefinitions.Clear();
            grdSeatingView.Children.Clear();

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                var grdRow = new RowDefinition();
                grdRow.Height = 50;

                grdSeatingView.RowDefinitions.Add(grdRow);

                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    var grdColumn = new ColumnDefinition();
                    grdColumn.Width = 50;

                    grdSeatingView.ColumnDefinitions.Add(grdColumn);

                    var text = seatingChart[row, column].Name;

                    var seatLabel = new Label();
                    seatLabel.Text = text;
                    seatLabel.HorizontalOptions = LayoutOptions.Center;
                    seatLabel.VerticalOptions = LayoutOptions.Center;
                    seatLabel.BackgroundColor = Color.Parse("#333388");
                    seatLabel.Padding = 10;

                    if (seatingChart[row, column].Reserved == true)
                    {
                        //Change the color of this seat to represent its reserved.
                        seatLabel.BackgroundColor = Color.Parse("#883333");

                    }

                    Grid.SetRow(seatLabel, row);
                    Grid.SetColumn(seatLabel, column);
                    grdSeatingView.Children.Add(seatLabel);

                }
            }
        }

        //Assign to Team 1 Member
        private async void ButtonReserveRange(object sender, EventArgs e)
        {
            var seatRange = await DisplayPromptAsync("Enter Seat Range", "Enter the starting and ending seat(ex., A1:A4)");

            if (string.IsNullOrWhiteSpace(seatRange) || !seatRange.Contains(":"))
            {
                await DisplayAlert("Error", "Invalid input format. Please enter in the format A1:A4", "Ok");
                return;
            }

            //Split input to get start and end seat
            var seats = seatRange.Split(':');
            if (seats.Length != 2)
            {
                await DisplayAlert("Error", "Invalid format. Please use 'A1:A4'.", "Ok");
                return;
            }

            string startSeat = seats[0].Trim();
            string endSeat = seats[1].Trim();

            int startRow = -1, startColumn = -1;
            int endRow = -1, endColumn = -1;

            //Find start and end seat position
            for (int i = 0; i < seatingChart.GetLength(0); i++)
            {
                for (int j = 0; j < seatingChart.GetLength(1); j++)
                {
                    if (seatingChart[i, j].Name == startSeat)
                    {
                        startRow = i;
                        startColumn = j;
                    }
                    if (seatingChart[i, j].Name == endSeat)
                    {
                        endRow = i;
                        endColumn = j;
                    }
                }
            }

            //Check if both seats were found
            if (startRow == -1 || startColumn == -1 || endRow == -1 || endColumn == -1)
            {
                await DisplayAlert("Error", "One or both seats were not found.", "Ok");
                return;
            }

            //Make sure the range is valid
            if (startRow == endRow && startColumn <= endColumn)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    if (seatingChart[startRow, j].Reserved)
                    {
                        await DisplayAlert("Error", "One or more seats in this range are already reserved.", "Ok");
                        return;
                    }
                }

                //If all seats are available reserve them
                for (int j = startColumn; j <= endColumn; j++)
                {
                    seatingChart[startRow, j].Reserved = true;
                }

                await DisplayAlert("Success", $"Seats {startSeat} to {endSeat} have been reserved!", "Ok");
                RefreshSeating();
            }
            else
            {
                await DisplayAlert("Error", "Invalid seat range. Ensure the seats are in the same row.", "Ok");
            }
        }

        //Assign to Team 2 Member - Grant West
        private async void ButtonCancelReservation(object sender, EventArgs e)
        {
            //Enter seat number for cancellation
            var seat = await DisplayPromptAsync("Cancel Reservation", "Enter the seat number to cancel reservation:");

            if (seat != null)
            {
                //Loop through the seatingChart to find the seat
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        //Check if the current seat matches the input
                        if (seatingChart[i, j].Name == seat)
                        {
                            //If the seat is reserved, cancel the reservation
                            if (seatingChart[i, j].Reserved)
                            {
                                seatingChart[i, j].Reserved = false;
                                await DisplayAlert("Reservation Canceled", $"Seat {seat} has been released.", "Ok");
                                RefreshSeating();
                            }
                            else
                            {
                                await DisplayAlert("Error", "Seat is not reserved.", "Ok");
                            }
                            return;
                        }
                    }
                }

                await DisplayAlert("Error", "Seat was not found.", "Ok");
            }

        }

        //Assign to Team 3 Member
        private void ButtonCancelReservationRange(object sender, EventArgs e)
        {

        }

        //Assign to Team 4 Member - Nathaniel Bullock
        private async void ButtonResetSeatingChart(object sender, EventArgs e)
        {
            // Go though each row and column in the seating chart and count the reservations
            // This is used only for displaying how many reservations are/to be cleared.
            int TotalReserved = 0;

            for (int i = 0; i < seatingChart.GetLength(0); i++)
            {
                for (int j = 0; j < seatingChart.GetLength(1); j++)
                {
                    if (seatingChart[i, j].Reserved == true)
                    {
                        TotalReserved++;
                    }
                }
            }

            // Ask for a conformation, displaying how many reservations will be cleared.
            bool confirm = await DisplayAlert("Reset Seating chart", $"Are you sure you want to reset the seating chart? {TotalReserved} reservations will be cleared!", "Yes", "No");

            // If "No" is clicked, Seats will not be cleared.
            if (!confirm)
            {
                return;
            }
            else
            {

                // Go through each row and column in the seating chart and clear reservations
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        seatingChart[i, j].Reserved = false;
                    }
                }

                // Refresh the seating view
                RefreshSeating();

                await DisplayAlert("Reset Seating Chart", $"All {TotalReserved} seat reservations have been cleared.", "Ok");


            }
        }

    }
}
