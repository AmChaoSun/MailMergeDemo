# MailMergeDemo
A small "mail-merge" solution that lets a user upload an email template and upload a list of personalisation variables (e.g. {{firstname}}, {{email}}, {{city}}).

Alert:
{{recipient}} is required in the csv for sending emails, sample csv is attached

sample email template:
Title : Test Email To {{recipient}}
Body : hello {{firstname}}, your email is {{email}}
