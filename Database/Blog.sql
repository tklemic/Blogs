DROP VIEW app_user_v CASCADE;

DROP VIEW post_v CASCADE;

DROP VIEW comment_v CASCADE;

DROP TABLE app_user CASCADE;

DROP TABLE role CASCADE;

DROP TABLE request CASCADE;

DROP TABLE comment CASCADE;


CREATE TABLE app_user (
    id                  SERIAL PRIMARY KEY,
    email               varchar(255),
    pass               varchar(255),
    name                 varchar(255),
    surname             varchar(255),
    date_of_birth       DATE,
    role_id            int NOT NULL,
    request_id          int NOT NULL,
    status              varchar(255)
); 

CREATE TABLE role (
    id                  SERIAL PRIMARY KEY,
    type_of_role                varchar(255)
);

CREATE TABLE request (
    id                  SERIAL PRIMARY KEY,
    request_status                varchar(255)
);

CREATE TABLE comment(
    id                  SERIAL PRIMARY KEY,
    content             varchar(255),
    app_user_id            int NOT NULL,
    post_id           int NOT NULL,
    date               DATE    
);

CREATE TABLE post(
    id                  SERIAL PRIMARY KEY,
    title              varchar(255),
    content             varchar(255),
    date               DATE,
    app_user_id         int NOT NULL
);



ALTER TABLE app_user
    ADD CONSTRAINT app_user_role_id FOREIGN KEY ( role_id )
        REFERENCES role ( id );

ALTER TABLE app_user
    ADD CONSTRAINT app_user_request_id FOREIGN KEY ( request_id )
        REFERENCES request ( id );

ALTER TABLE post
    ADD CONSTRAINT post_app_user_id FOREIGN KEY ( app_user_id )
        REFERENCES app_user ( id );

ALTER TABLE comment
    ADD CONSTRAINT comment_post_id FOREIGN KEY ( post_id )
        REFERENCES post ( id );

ALTER TABLE comment
    ADD CONSTRAINT comment_app_user_id FOREIGN KEY ( app_user_id )
        REFERENCES app_user ( id );

insert into role (id, type_of_role) values (1, 'admin');
insert into role (id, type_of_role) values (2, 'guest');

insert into request (id, request_status) values (1, 'In progress');
insert into request (id, request_status) values (2, 'Granted');
insert into request (id, request_status) values (3, 'Declined');

insert into app_user (email, pass, name, surname, date_of_birth, role_id, request_id, status) values ('admin@gmail.com', 'Admin1', 'Tomislav', 'Klemić', '1996-06-27', 1, 2, 'Active');
insert into app_user (email, pass, name, surname, date_of_birth, role_id, request_id, status) values ('guest11', 'Guest11', 'Pero', 'Perić', '1988-08-27', 2, 2, 'Active');
insert into app_user (email, pass, name, surname, date_of_birth, role_id, request_id, status) values ('ad', 'Admin2', 'Tomo', 'Tomić', '1988-08-27', 1, 2, 'Active');
insert into app_user (email, pass, name, surname, date_of_birth, role_id, request_id, status) values ('guest77@gmail.com', 'Guest77', 'Marko', 'Marić', '1988-08-27', 2, 1, 'Inactive');


insert into post (title, content, date, app_user_id) values ('Hobbit', 'Bilbo Baggins.','2022-06-27', 1);
insert into post (title, content, date, app_user_id) values ('Lord of the rings', 'They threw the ring into the fires of Mouont Doom', '2020-05-14', 3);

insert into comment (content, app_user_id, post_id, date) values ('Indeed they threw it.', 2, 2, '2022-08-08');


CREATE OR REPLACE VIEW app_user_v (email, pass, name, surname, date_of_birth, role, request) as
SELECT
    app_user.email,
    app_user.pass,
    app_user.name,
    app_user.surname,
    app_user.date_of_birth,
    role.type_of_role AS role,
    request.request_status AS request
FROM
         app_user
    left JOIN role ON role.id = app_user.role_id
    left JOIN request ON request.id = app_user.request_id;

CREATE OR REPLACE VIEW post_v (id, title, content, date, name, surname, status, email, app_user_id) as
SELECT
    post.id,
    post.title,
    post.content,
    post.date,
    app_user.name AS name,
    app_user.surname AS surname,
    app_user.status AS status,
    app_user.email AS email,
    post.app_user_id
FROM
         post
    left JOIN app_user ON app_user.id = post.app_user_id;

CREATE OR REPLACE VIEW comment_v (id, content, name, surname, post_id, app_user_id, date) as
SELECT
    comment.id,
    comment.content,
    app_user.name AS name,
    app_user.surname AS surname,
    comment.post_id,
    comment.app_user_id,
    comment.date
FROM
         comment
    left JOIN app_user ON app_user.id = comment.app_user_id;