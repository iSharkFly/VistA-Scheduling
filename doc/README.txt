BSDX 1.41 for WorldVista

This is a Scheduling GUI package. 

Pre-requisites:
FM 22
Kernel 8
XB 3 or above
SD 5.3
SD 5.3 patch 11310 (to fix a problem with the code)
BMX 2.2 (now required)

For a Virgin WorldVistA 6-08 release install the following in this order:
1. XB 4                 (see https://trac.opensourcevista.net/browser/IHS-VA_UTILITIES-XB/tag/rel_20091207) 
2. BMX 2.2              (see https://trac.opensourcevista.net/browser/BMXNET_RPMS_dotNET_UTILITIES-BMX/trunk/k)
3. XWB 1.1 patch 113102 (see https://trac.opensourcevista.net/browser/BMXNET_RPMS_dotNET_UTILITIES-BMX/trunk/k)
4. BSDX 1.41             (see https://trac.opensourcevista.net/browser/Scheduling/trunk/kids)
5. SD 5.3 patch 11310   (see https://trac.opensourcevista.net/browser/Scheduling/trunk/kids)

Client (download from https://trac.opensourcevista.net/browser/Scheduling/trunk/cs/bsdx0200GUISourceCode/bin/Release):
ClinicalScheduling.exe
BMXNet22.dll

Client does not need installation. Both files have to be located in the same folder.

For users who used a previous version, you only need to download and install BSDX 1.41 kids and ClinicalScheduling.exe plus the BMXNet22.dll library. If BMX Server version is outdated, you need to install the new version.

Post-installation tasks:
After installation, complete the following tasks to configure Windows Scheduling:
1. Using VISTA tools, assign the BSDXMENU security key. All scheduling users
must have the BSDXZMENU security key.
2. Users who are also scheduling managers must have the BSDXZMGR key
assigned. The BSDXZMGR key permits access to the Scheduling Management
menu in the client application. This menu supports the creation of clinics and
clinic groups, assignment of users to clinics, designation of user access, and other management activities. For further details, see the User Manual.
3. Make the BMXRPC and BSDXPRC menu options available to scheduling users.
These options must be somewhere in the user’s path, either as a secondary option
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
- Remaining slots calculation does not work properly if you have more than 1 slot per access block.
- Find Appointments function is not operational in Scheduling GUI
- IDs in Scheduling GUI reflect the HRN not the Primary ID
- Cannot cancel a walk-in appointment in Scheduling...
- No handling of invalid access code when saving access slots.
- Saving access slots causes program to freeze until it's done: need to be Async
- No Ctrl-C & Ctrl-V handling
- No Insert & Delete button handling
- After applying access templates, the access blocks screen doesn't refresh itself.
- Logic to remove old access blocks does not work. Only workaround is to remove them manually.
- Transactions fail when a restart is attempted (Oroville's system only).
- Grids don't respond to mouse wheel movement.
- Default open location for Apply Template is inappropriate. Should be the last folder navigated.
- Drag and drop plus delete Message Boxes on Access Blocks editor are unnecessary.
- Appointment drag and drop to the same time at a different clinic doesn't work (complains that the patient already has an appointment at this time).
- Appointment drap and drop between different windows doesn't cancel the original appointment.


Other Bugs:
Put them on the trac server where you got this software.

