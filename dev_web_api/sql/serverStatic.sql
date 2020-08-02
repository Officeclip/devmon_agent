BEGIN TRANSACTION;

INSERT INTO 
    "groups" 
    ("id","server_guid") 
VALUES 
    (1,'ad137570-68c8-481c-ad22-f96e3cf41ea5');

INSERT INTO 
    "users" 
    ("user_id","email_address", "password", "email_opt") 
VALUES 
    (1,'skd@officeclip.com', 'abcd1234@A', 1);

COMMIT;