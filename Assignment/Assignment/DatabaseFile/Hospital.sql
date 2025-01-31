toc.dat                                                                                             0000600 0004000 0002000 00000010450 14612203032 0014430 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        PGDMP   
    .                |            Hospital    16.1    16.1     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false         �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false         �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false         �           1262    26240    Hospital    DATABASE     }   CREATE DATABASE "Hospital" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';
    DROP DATABASE "Hospital";
                postgres    false         �            1259    26253    doctor    TABLE       CREATE TABLE public.doctor (
    doctorid integer NOT NULL,
    createddate timestamp without time zone NOT NULL,
    specialist character varying(256),
    firstname character varying(255),
    lastname character varying(255),
    email character varying(255)
);
    DROP TABLE public.doctor;
       public         heap    postgres    false         �            1259    26252    doctor_doctorid_seq    SEQUENCE     �   ALTER TABLE public.doctor ALTER COLUMN doctorid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.doctor_doctorid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    216         �            1259    26259    patient    TABLE     �  CREATE TABLE public.patient (
    id integer NOT NULL,
    firstname character varying(100),
    lastname character varying(100),
    doctorid integer NOT NULL,
    age integer,
    email character varying(50) NOT NULL,
    phoneno character varying(23),
    disease character varying(256),
    isdeleted boolean,
    createddate timestamp without time zone,
    modifieddate timestamp without time zone,
    gender character varying
);
    DROP TABLE public.patient;
       public         heap    postgres    false         �            1259    26258    patient_id_seq    SEQUENCE     �   ALTER TABLE public.patient ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.patient_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    218         �          0    26253    doctor 
   TABLE DATA           _   COPY public.doctor (doctorid, createddate, specialist, firstname, lastname, email) FROM stdin;
    public          postgres    false    216       4842.dat �          0    26259    patient 
   TABLE DATA           �   COPY public.patient (id, firstname, lastname, doctorid, age, email, phoneno, disease, isdeleted, createddate, modifieddate, gender) FROM stdin;
    public          postgres    false    218       4844.dat �           0    0    doctor_doctorid_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public.doctor_doctorid_seq', 2, true);
          public          postgres    false    215         �           0    0    patient_id_seq    SEQUENCE SET     =   SELECT pg_catalog.setval('public.patient_id_seq', 19, true);
          public          postgres    false    217         V           2606    26257    doctor pk_doctor 
   CONSTRAINT     T   ALTER TABLE ONLY public.doctor
    ADD CONSTRAINT pk_doctor PRIMARY KEY (doctorid);
 :   ALTER TABLE ONLY public.doctor DROP CONSTRAINT pk_doctor;
       public            postgres    false    216         X           2606    26265    patient pk_patient 
   CONSTRAINT     P   ALTER TABLE ONLY public.patient
    ADD CONSTRAINT pk_patient PRIMARY KEY (id);
 <   ALTER TABLE ONLY public.patient DROP CONSTRAINT pk_patient;
       public            postgres    false    218         Y           2606    26266    patient fk_patient    FK CONSTRAINT     y   ALTER TABLE ONLY public.patient
    ADD CONSTRAINT fk_patient FOREIGN KEY (doctorid) REFERENCES public.doctor(doctorid);
 <   ALTER TABLE ONLY public.patient DROP CONSTRAINT fk_patient;
       public          postgres    false    4694    216    218                                                                                                                                                                                                                                4842.dat                                                                                            0000600 0004000 0002000 00000000200 14612203032 0014234 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        1	2024-03-02 00:00:00	Health Diease	Yash	Variya	yashvariya@gmail.com
2	2024-04-02 00:00:00	Surgor	John	John	john@gmail.com
\.


                                                                                                                                                                                                                                                                                                                                                                                                4844.dat                                                                                            0000600 0004000 0002000 00000001323 14612203032 0014245 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        3	Parth	Trivedi	1	20	Parth@gmail.com	9157568271	Numeroligst	t	\N	\N	\N
4	Parth	Trivedi	1	20	Parth@gmail.com	9157568271	Numeroligst	t	\N	\N	\N
5	Parth	Trivedi	1	20	Parth@gmail.com	9157568271	Numeroligst	t	\N	\N	\N
6	Tirth	Patel	1	20	tirth@gmail.com	9157568271	Numeroligst	t	\N	\N	\N
1	Pratik1	Variya	1	23	pratik@gmail.com	915856824	Cardologist	\N	2024-03-02 00:00:00	\N	Male
8	John	Week	1	69	john@gmail.com	5565556545	Surgeon	\N	\N	\N	Other
7	Vatsal	Gadoya	1	20	vatsal@gmail.com	9157568271	Numeroligst	\N	\N	\N	Male
9	Lil	John	1	34	lil@gmail.com	9157568271	Surgeon	\N	\N	\N	Female
10	U	L	1	20	ul@gmail.com	9157568271	Numeroligst	t	\N	\N	Male\n
12	Parth	Trivedi	1	20	Parth@gmail.com	9157568271	Numeroligst	\N	\N	\N	Male
\.


                                                                                                                                                                                                                                                                                                             restore.sql                                                                                         0000600 0004000 0002000 00000010457 14612203032 0015364 0                                                                                                    ustar 00postgres                        postgres                        0000000 0000000                                                                                                                                                                        --
-- NOTE:
--
-- File paths need to be edited. Search for $$PATH$$ and
-- replace it with the path to the directory containing
-- the extracted data files.
--
--
-- PostgreSQL database dump
--

-- Dumped from database version 16.1
-- Dumped by pg_dump version 16.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE "Hospital";
--
-- Name: Hospital; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "Hospital" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_India.1252';


ALTER DATABASE "Hospital" OWNER TO postgres;

\connect "Hospital"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: doctor; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.doctor (
    doctorid integer NOT NULL,
    createddate timestamp without time zone NOT NULL,
    specialist character varying(256),
    firstname character varying(255),
    lastname character varying(255),
    email character varying(255)
);


ALTER TABLE public.doctor OWNER TO postgres;

--
-- Name: doctor_doctorid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.doctor ALTER COLUMN doctorid ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.doctor_doctorid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: patient; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.patient (
    id integer NOT NULL,
    firstname character varying(100),
    lastname character varying(100),
    doctorid integer NOT NULL,
    age integer,
    email character varying(50) NOT NULL,
    phoneno character varying(23),
    disease character varying(256),
    isdeleted boolean,
    createddate timestamp without time zone,
    modifieddate timestamp without time zone,
    gender character varying
);


ALTER TABLE public.patient OWNER TO postgres;

--
-- Name: patient_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.patient ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.patient_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: doctor; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.doctor (doctorid, createddate, specialist, firstname, lastname, email) FROM stdin;
\.
COPY public.doctor (doctorid, createddate, specialist, firstname, lastname, email) FROM '$$PATH$$/4842.dat';

--
-- Data for Name: patient; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.patient (id, firstname, lastname, doctorid, age, email, phoneno, disease, isdeleted, createddate, modifieddate, gender) FROM stdin;
\.
COPY public.patient (id, firstname, lastname, doctorid, age, email, phoneno, disease, isdeleted, createddate, modifieddate, gender) FROM '$$PATH$$/4844.dat';

--
-- Name: doctor_doctorid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.doctor_doctorid_seq', 2, true);


--
-- Name: patient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.patient_id_seq', 19, true);


--
-- Name: doctor pk_doctor; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.doctor
    ADD CONSTRAINT pk_doctor PRIMARY KEY (doctorid);


--
-- Name: patient pk_patient; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.patient
    ADD CONSTRAINT pk_patient PRIMARY KEY (id);


--
-- Name: patient fk_patient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.patient
    ADD CONSTRAINT fk_patient FOREIGN KEY (doctorid) REFERENCES public.doctor(doctorid);


--
-- PostgreSQL database dump complete
--

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 