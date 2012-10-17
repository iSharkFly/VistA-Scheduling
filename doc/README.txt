BSDX 1.7 for WorldVista
Licensed under LGPL. http://www.gnu.org/licenses/lgpl-2.1.html


This is a Scheduling GUI package for WorldVistA.

Pre-requisites:
FM 22 and above
Kernel 8 and above
XB 3 and above
SD 5.3 
	
SD 5.3 patch 11310 (to fix a problem with the code from the VA that IHS has fixed)
	
BMX 4 For a Virgin WorldVistA 6-08/3-09 release install the following kids builds in this order:

		1. XB 4                 (in https://trac.opensourcevista.net/svn/IHS-VA_UTILITIES-XB/tag/rel_20091207/xb0400.k.WV.zip) 

		2. BMX 4              (in https://trac.opensourcevista.net/svn/BMXNET_RPMS_dotNET_UTILITIES-BMX/branch/BMX41000/kids/bmx_041000.kids)

		3. XWB 1.1 patch 113102 (in https://trac.opensourcevista.net/svn/BMXNET_RPMS_dotNET_UTILITIES-BMX/tag/2.3/bmx_0230.zip)

		4. BSDX 1.6                    (in https://trac.opensourcevista.net/svn/Scheduling/tag/1.7/bsdx_0170.zip)

		5. SD 5.3 patch 11310   (in https://trac.opensourcevista.net/svn/Scheduling/tag/1.7/bsdx_0170.zip)


		Client (in https://trac.opensourcevista.net/svn/Scheduling/tag/1.7/bsdx_0170.zip):
			
ClinicalScheduling.exe
			BMXNET40.dll
			
BMXWIN40.dll
			PrintPreview.dll
			To enable Arabic Support, use the ar folder's contents as is in the zip file.

Client does not need installation. 
			Both files have to be located in the same folder.
		If BMX Server version is outdated, you need to install the new version.


	
-Post-installation tasks:
After installation, complete the following tasks to configure Windows Scheduling:

	1. Using VISTA tools, assign the BSDXMENU security key. All scheduling users
must have the BSDXZMENU security key.

	2. Users who are also scheduling managers must have the BSDXZMGR key
assigned. The BSDXZMGR key permits access to the Scheduling Management
menu in the client application. This menu supports the creation of clinics and
clinic groups, assignment of users to clinics, designation of user access, and other management activities. For further details, see the User Manual.
	
3. Make the BSDXPRC menu options available to scheduling users.
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

	The program is in a sort of an intermediate state; it doesn't edit PIMS clinics directly, but can be linked to them if you want appointments and check-ins to show up in PIMS. 	This program can work without ever being linked to PIMS clinics.


	If you need to use PIMS clinics, here is how you do the set-up:

		0. First, make sure you have BSDXRPC in your menu path and that you have BSDXZMGR for Sched GUI set-up and DG SUPERVISOR TO set up PIMS clinics on roll and scroll.

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

		8. Create slots for each of the clinics. 
		You can save them as files and re-use them.



Known Bugs:
	
- Users booking appointments at exactly the same time for the same clinic doesn't work properly (concurrency issues).
	
- IDs in Scheduling GUI reflect the HRN not the Primary ID

	- No handling of invalid access code when saving access slots.

	- No Ctrl-C & Ctrl-V handling
	
- Appointment drag and drop to the same time at a different clinic doesn't work (complains that the patient already has an appointment at this time).
	
- Appointment drap and drop between different windows doesn't cancel the original appointment.
	
- Rebooking under certain conditions causes system hangs. 

Two issues: Program doesn't increment requested available appointment from VISTA, and doesn't deal properly with an appointment that doesn't have a access type (ie an appointment that is not in an Access block).

	
- When making a walk-in appointment, it lets you cancel check-in by pressing the cancel button, yet walk-in continues to be made.
	
- If user has no access to any schedules but has access to application (has BSDXZMENU key), opening the appointment menu causes a crash.
	
- Clipboard takes the same patient multiple times over.
	
- Patient search is slow with common names due to slow performance of FIND^DIC
	
- If DB has a deleted PIMS appointment but the BSDX appointment is still there, the appointment will not get deleted on a cancel.



Other Bugs:
Put them on the trac server where you got this software.

Enhancement requests:
	
- Show Appointment Status in patient information windows. In other words show if the appointment is a future, checked-in, checked-out, or canceled status. (Oroville)
	
- Allow all users to access all clinics by default (EHS/PHH)

	- Summary Schedule report based on Clinic Schedule report. Remove Phone, Address, Appointment Made By, and on fields.
	
- Alert user if a patient already has an appointment in the same clinic for today.
	
- Double-click should open make appoitment

	- Double-click on appointment should edit appointment

	- Make the manager for the division not for the Data Base <Najjar>
	
- Map the Clerks (Users) to the Groups not the clinics <Najjar>
	
- Add New Key to the system; allow the manager to map users to the clinics without having access to add and edit the clinics
	- In the Re-book function; system should inform the users if there is no available clinic, not just do nothing.
	
- Select the provider from the Check-in option should print the provider on the routing slip (1.5: prints; but not stored in DB)
	
- Have an overbook limit and it can be edited as in VistA Scheduling
	
- Show a indicator if a patient got checked out or not.
	
- Generate a report from the system, tell us about the Check-in & Check-out status
	
- If the I close the Last Windows in the clinical Scheduling GUI the system should ask me "Are you sure to close this program?"

	- If I apply a new template to the clinic the system should ask me if I want to delete the previous one

	- If I inactivate the clinic in VistA it should reflects in GUI
	
- If we can make the users in groups. i.e. MRs group, OP clinics Group and map them to the resource groups
	
- Show today's column in a different color.
	
- Show Holiday columns in a different color.
	
- Be able to copy appointments in mass from one clinic to another.
	
- Manager functions: Send message and Shutdown sends message or shuts down all users, not just individual users.

	- SMS to remind patients of appointments.
