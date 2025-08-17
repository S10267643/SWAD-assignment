using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class CTLPickupTime
    {
        private readonly FoodStall _stall;
        private readonly Student _student;
        private readonly UIPickupTime _ui;
        private readonly Order _order;

        public CTLPickupTime(FoodStall stall, Student student, Order order)
        {
            _stall = stall;
            _student = student;
            _ui = new UIPickupTime();
            _order = order;
        }

        public void SelectPickupTime(bool isModification = false)
        {
            bool retry = true;
            while (retry)
            {
                // Display available time slots
                _ui.DisplayAvailableTimeSlots(_stall, _student is Priority);

                // Prompt for time slot selection
                int slotId = _ui.PromptTimeSlotSelection();

                // Handle cancellation
                if (slotId == 0)
                {
                    _ui.DisplayModificationCancelled();
                    return;
                }

                // Validate slot ID
                if (slotId < 1)
                {
                    Console.WriteLine("Invalid slot selection. Please try again.");
                    continue;
                }

                try
                {
                    // Check slot availability
                    DateTime slotTime = _stall.GetTimeSlotTime(slotId);
                    bool isAvailable = _stall.IsTimeSlotAvailable(slotId) || _student is Priority;

                    if (!isAvailable)
                    {
                        _ui.DisplaySlotUnavailableMessage();

                        // Suggest alternative slots
                        var alternatives = FindAlternativeSlots(slotId);
                        if (alternatives.Any())
                        {
                            _ui.DisplayAlternativeSlots(alternatives);
                        }
                        else
                        {
                            Console.WriteLine("No alternative slots available at this time.");
                        }
                        continue;
                    }

                    // Confirm selection
                    bool confirmed = _ui.ConfirmTimeSlotSelection(slotId, slotTime, isModification);
                    if (!confirmed)
                    {
                        if (isModification)
                        {
                            _ui.DisplayModificationCancelled();
                            return;
                        }
                        continue;
                    }

                    // Process the time slot selection
                    ProcessTimeSlotSelection(slotId, slotTime, isModification);
                    retry = false;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Invalid time slot selected. Please try again.");
                }
            }
        }

        private List<(int SlotId, DateTime Time)> FindAlternativeSlots(int originalSlotId, int count = 3)
        {
            var alternatives = new List<(int, DateTime)>();
            int range = 5; // Look 5 slots before and after

            // Check later slots first
            for (int i = originalSlotId + 1; i <= originalSlotId + range && alternatives.Count < count; i++)
            {
                try
                {
                    if (_stall.IsTimeSlotAvailable(i))
                    {
                        DateTime time = _stall.GetTimeSlotTime(i);
                        alternatives.Add((i, time));
                    }
                }
                catch (ArgumentException) { /* Skip invalid slots */ }
            }

            // Check earlier slots if we still need more
            for (int i = originalSlotId - 1; i >= originalSlotId - range && alternatives.Count < count; i--)
            {
                try
                {
                    if (_stall.IsTimeSlotAvailable(i))
                    {
                        DateTime time = _stall.GetTimeSlotTime(i);
                        alternatives.Insert(0, (i, time)); // Keep in chronological order
                    }
                }
                catch (ArgumentException) { /* Skip invalid slots */ }
            }

            return alternatives;
        }

        private void ProcessTimeSlotSelection(int slotId, DateTime slotTime, bool isModification)
        {
            // Book the time slot
            bool booked = _stall.BookTimeSlot(slotId, _student is Priority);

            if (booked)
            {
                // Update student's time slot
                _student.PickUpTimeSlot = slotId;

                // Generate QR code
                string qrCode = GenerateQRCode(_order.OrderId, slotId);

                // Display confirmation
                _ui.DisplayConfirmationMessage(slotId, slotTime, isModification);
                _ui.DisplayQRCode(qrCode);

                // For modifications, apply fee
                if (isModification)
                {
                    ApplyChangeFee();
                }
            }
            else
            {
                Console.WriteLine("Failed to book the time slot. Please try again.");
            }
        }

        private string GenerateQRCode(int orderId, int slotId)
        {
            // In a real implementation, this would generate an actual QR code
            // For this example, we'll just return a string representation
            return $"QR-{orderId}-{slotId}-{DateTime.Now:yyyyMMddHHmm}";
        }

        private void ApplyChangeFee()
        {
            // In a real implementation, this would charge the student's account
            Console.WriteLine("A $2 change fee has been applied to your account.");
        }

        public void HandleTimeSlotModification()
        {
            if (_student.PickUpTimeSlot == 0)
            {
                Console.WriteLine("You don't have a current pickup time to modify.");
                return;
            }

            // Show current time slot
            try
            {
                DateTime currentTime = _stall.GetTimeSlotTime(_student.PickUpTimeSlot);
                Console.WriteLine($"\nYour current pickup time: {currentTime:hh:mm tt}");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("\nYour current pickup time is no longer valid.");
            }

            // Proceed with modification
            SelectPickupTime(true);
        }
    }
}