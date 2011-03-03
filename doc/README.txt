BSDX 1.42 for WorldVista
Licensed under LGPL.

This is a Scheduling GUI package. 

Pre-requisites:
FM 22
Kernel 8
XB 3 or above
SD 5.3
SD 5.3 patch 11310 (to fix a problem with the code)
BMX 2.21

For a Virgin WorldVistA 6-08 release install the following in this order:
1. XB 4                 (see https://trac.opensourcevista.net/browser/IHS-VA_UTILITIES-XB/tag/rel_20091207) 
2. BMX 2.21              (see https://trac.opensourcevista.net/browser/BMXNET_RPMS_dotNET_UTILITIES-BMX/trunk/k)
3. XWB 1.1 patch 113102 (see https://trac.opensourcevista.net/browser/BMXNET_RPMS_dotNET_UTILITIES-BMX/trunk/k)
4. BSDX 1.42            (see https://trac.opensourcevista.net/browser/Scheduling/trunk/kids)
5. SD 5.3 patch 11310   (see https://trac.opensourcevista.net/browser/Scheduling/trunk/kids)

Client (download from https://trac.opensourcevista.net/browser/Scheduling/trunk/cs/bsdx0200GUISourceCode/bin/Release):
ClinicalScheduling.exe
BMXNet22.dll

Client does not need installation. Both files have to be located in the same folder.

For users who used a previous version, you only need to download and install BSDX 1.42 kids and ClinicalScheduling.exe plus the BMXNet22.dll library. If BMX Server version is outdated, you need to install the new version.

Post-installation tasks:
After installation, complete the following tasks to configure Windows Scheduling:
1. Using VISTA tools, assign the BSDXMENU security key. All scheduling users
must have the BSDXZMENU security key.
2. Users who are also scheduling managers must have the BSDXZMGR key
assigned. The BSDXZMGR key permits access to the Scheduling Management
menu in the client application. This menu supports the creation of clinics and
clinic groups, assignment of users to clinics, designation of user access, and other management activities. For further details, see the User Manual.
3. Make the BMXRPC and BSDXPRC menu options available to scheduling users.
These options must be somewhere in the user's path, either as a secondary option
or as members of a menu to which the user has access.
4. Use Scheduling Management to configure 
a. Resources (clinics)
b. Users to work with those clinics
c. Resource Groups, then add the clinics to the resource groups.
d. Access Types
e. Access Type Groups
(see below for more details).
5. Restart the program, and edit the resource availablility by right clicking on it in the tree.

If you don't do these steps, the program won't work.
See the User Manual for detailed instructions. 

Detailed Clinic Configuration Instructions:
The program is in a sort of an intermediate state; it doesn't edit PIMS clinics directly, but can be linked to them if you want appointments and check-ins to show up in PIMS. This program can work without ever being linked to PIMS clinics.

If you need to use PIMS clinics, here is how you do the set-up:
0. First, make sure you have BMXRPC and BSDXRPC in your menu path and that you have BSDXZMGR for Sched GUI set-up and DG SUPERVISOR TO set up PIMS clinics on roll and scroll.
1. Create the PIMS clinics using SDBUILD menu in VISTA. The GUI uses the following fields from the Hospital Location file
 - Clinic Name
 - Inactivation Date/Reactivation Date for deciding whether to display it.
 - Default Provider multiple for populating the default providers
 - Division (not currently used, but will be in the future)
2. Create the resources in the GUI, and tie each of them to the clinics as needed.
3. For each resource, you need to add authorized users. These users must hold the BSDXZMENU key or else, they won't show up. You may see users with XUPROGMODE. These will always show up.
4. Create Resource Groups, and add resources to them. Without this, the tree on the side won't show up; and without this tree, users cannot select a schedule.
5. Set-up At least 1 access type
6. Set-up At least 1 access group
7. Restart 
8. Create slots for each of the clinics. You can save them as files and re-use them.

Known Bugs:
- Users booking appointments at exactly the same time for the same clinic doesn't work properly (concurrency issues).
- Various usability issues that are apparent in the program. E.g. you need to click before you right click, drag and drop has no visiual assist to show what you are dragging and where to, etc.
- Remaining slots calculation does not work properly if you have more than 1 slot per access block (Najjar says that it doesn't work for 1 slot either if you change the time scale). (being fixed in v 1.5).
- Find Appointments function is not operational in Scheduling GUI (fixed in 1.5)
- IDs in Scheduling GUI reflect the HRN not the Primary ID
- Cannot cancel a walk-in appointment in Scheduling... (fixed in 1.5)
- No handling of invalid access code when saving access slots.
- No Ctrl-C & Ctrl-V handling
- No Insert & Delete button handling
- Grids don't respond to mouse wheel movement. (fixed in 1.5)
- Appointment drag and drop to the same time at a different clinic doesn't work (complains that the patient already has an appointment at this time).
- Appointment drap and drop between different windows doesn't cancel the original appointment.
- Rebooking under certain conditions causes system hangs. Two issues: Program doesn't increment requested available appointment from VISTA, and doesn't deal properly with an appointment that doesn't have a access type (ie an appointment that is not in an Access block).
- Speed issues (see below) <better in 1.5>
- Date on Appt List header in Arabic doesn't show up correctly.
- Error massages should be clearer for the end Users <fixed in 1.5>
- Event raise for clinic groups raises the name of the group not the clinic. The receiving end in the C# code compares and checks for the clinic.
- When making a walk-in appointment, it lets you cancel check-in by pressing the cancel button, yet walk-in continues to be made.
- If user has no access to any schedules but has access to application (has BSDXZMENU key), opening the appointment menu causes a crash.
- Shortcut server/port is not overridden when cancel, retry and enter them manually.


Other Bugs:
Put them on the trac server where you got this software.

Enhancement requests:
- Show Appointment Status in patient information windows. In other words show if the appointment is a future, checked-in, checked-out, or canceled status. (Oroville)
- Allow all users to access all clinics by default (EHS/PHH)
- Summary Schedule report based on Clinic Schedule report. Remove Phone, Address, Appointment Made By, and on fields.
- Alert user if a patient already has an appointment in the same clinic for today.
- Print Appointment Letter when Appointment is made (PHH Users workflow).
- Make printing an extensible module for end user customization (Oroville and PHH)
- Be able to print mutiple copies of a report (request of a PHH user).
- Appointment List print out doesn't show the date and time printed.
- Add patient order to the routing slip
- Double-click should open make appoitment
- Double-click on appointment should edit appointment
- Click right click issue on grid
- Add right-click on tvClinics to Print schedule for the clinic
- Print multiple copies of schedule for clinic.
- Add ability to Cancel Walk-in appointments <Najjar>
- Make the manager for the division not for the Data Base <Najjar>
- Map the Clerks (Users) to the Groups not the clinics <Najjar>
- Add New Key to the system; allow the manager to map users to the clinics without having access to add and edit the clinics
- In the Re-book function; system should inform the users if there is no available clinic, not just do nothing.
- Select the provider from the Check-in option should print the provider on the routing slip
- Make the reason for the cancellation editable can be configured site Specific
- Have an overbook limit and it can be edited as in VistA Scheduling
- Show a indicator if a patient got checked out or not.
- Generate a report from the system, tell us about the Check-in & Check-out status
- Change VistA Server should be only for the manager only
- If the I close the Last Windows in the clinical Scheduling GUI the system should ask me "Are you sure to close this program?"
- A tool that allow us to edit the schedule without go to each day to change
- If I apply a new template to the clinic the system should ask me if I want to delete the previous one
- Adding blocks in groups not individually
- If I inactivate the clinic in VistA it should reflects in GUI
- If we can make the users in groups. i.e. MRs group, OP clinics Group and map them to the resource groups
- Add an option to change the locale independent of the Windows locale (Khamis; PHH) <not doable>; changed date format instead in v 1.5
- Show today's column in a different color.
- Show Holiday columns in a different color.
- Be able to copy appointments in mass from one clinic to another.
- Manager functions: Send message and Shutdown sends message or shuts down all users, not just individual users.
- SMS to remind patients of appointments.

Speed issues:
- Loading takes a long time <solved in 1.5>
- Search <enter> Select <enter> <enter> takes a long time when adding appointment <solved in 1.5>
- Wait cursor when updating <solved in 1.5>
- Checkin takes a long time <solved in 1.5>
- Printing the routing slip takes a slightly longer than necessary time <not an issue in main version>
- Select patient dialog takes a tiny while to show up. <solved in 1.5>
- Date-change selection not optimal (calls server with each tiny change) <solved in 1.5>
