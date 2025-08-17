using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class UIPickupTime
    {
        public void DisplayAvailableTimeSlots(FoodStall stall, bool isPriority = false)
        {
            Console.Clear();
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("      SELECT PICKUP TIME SLOT        ");
            Console.WriteLine("══════════════════════════════════════\n");

            if (isPriority)
            {
                stall.DisplayTimeSlotsPriority();
            }
            else
            {
                stall.DisplayTimeSlots();
            }
        }

        public int PromptTimeSlotSelection()
        {
            Console.Write("\nEnter your preferred time slot number (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int slotId))
            {
                return slotId;
            }
            return -1; // Invalid input
        }

        public bool ConfirmTimeSlotSelection(int slotId, DateTime slotTime, bool isModification = false)
        {
            if (isModification)
            {
                Console.WriteLine($"\nYou have selected: Slot {slotId} at {slotTime:hh:mm tt}");
                Console.WriteLine("⚠️  Changing your time slot will incur a $2 fee");
                Console.Write("Do you confirm this change? (Y/N): ");
            }
            else
            {
                Console.WriteLine($"\nYou have selected: Slot {slotId} at {slotTime:hh:mm tt}");
                Console.Write("Do you confirm this selection? (Y/N): ");
            }

            string input = Console.ReadLine()?.Trim().ToUpper();
            return input == "Y";
        }

        public void DisplaySlotUnavailableMessage()
        {
            Console.WriteLine("\n❌ This time slot is no longer available.");
        }

        public void DisplayAlternativeSlots(List<(int SlotId, DateTime Time)> alternatives)
        {
            Console.WriteLine("\nHere are the closest available time slots:");
            foreach (var alt in alternatives)
            {
                Console.WriteLine($"- Slot {alt.SlotId} at {alt.Time:hh:mm tt}");
            }
        }

        public void DisplayConfirmationMessage(int slotId, DateTime slotTime, bool isModification = false)
        {
            if (isModification)
            {
                Console.WriteLine($"\n✅ Your pickup time has been changed to {slotTime:hh:mm tt}");
                Console.WriteLine("A $2 change fee has been applied to your account.");
            }
            else
            {
                Console.WriteLine($"\n✅ Time slot {slotId} at {slotTime:hh:mm tt} successfully booked!");
            }
        }

        public void DisplayQRCode(string qrCodeData)
        {
            Console.WriteLine("\nYour pickup QR code:");
            Console.WriteLine("════════════════════════");
            Console.WriteLine(qrCodeData);
            Console.WriteLine("════════════════════════");
            Console.WriteLine("Present this code at the food stall for pickup.");
        }

        public bool PromptForModification()
        {
            Console.Write("\nWould you like to modify your pickup time? (Y/N): ");
            string input = Console.ReadLine()?.Trim().ToUpper();
            return input == "Y";
        }

        public void DisplayModificationCancelled()
        {
            Console.WriteLine("\nTime slot modification cancelled.");
        }
    }
}