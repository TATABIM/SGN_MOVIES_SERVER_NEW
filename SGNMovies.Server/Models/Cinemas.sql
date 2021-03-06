USE SGNMOVIE
GO

--galaxy provider--
SET IDENTITY_INSERT Providers ON
insert into Providers(Id, Name, HostUrl) values(1,N'galaxy',N'http://www.galaxycine.vn')
SET IDENTITY_INSERT Providers OFF

--megastar cinemas--
--SET IDENTITY_INSERT Cinemas ON
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1009,N'HCMC - Crescent Mall',N'Lầu 5, Crescent Mall, Đại lộ Nguyễn Văn Linh, Phú Mỹ Hưng, Quận 7, TP. Hồ Chí Minh',N'(08) 5412-2222',10.74388,106.71092,N'',N'HCMC - Crescent Mall')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1006,N'HCMC - CT Plaza',N'Tầng 10, CT Plaza, 60A Trường Sơn, Phường 2, Quận Tân Bình, Tp. Hồ Chí Minh',N'+848 62 971 981',10.80951,106.66478,N'',N'HCMC - CT Plaza')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1004,N'HCMC - Hùng Vương Plaza',N'Tầng 7 | Hùng Vương Plaza, 126 Hùng Vương, Quận 5, Tp. Hồ Chí Minh',N'(84-8) 2 2220 388',10.76137,106.67480,N'',N'HCMC - Hùng Vương Plaza')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1007,N'HCMC - Parkson Paragon',N'Tầng 5, toà nhà Parkson Paragonm, 03 Nguyễn Lương Bằng, Quận 7, TP. Hồ Chí Minh',N'+848 5416 00 88',10.73023,106.72110,N'',N'HCMC - Parkson Paragon')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1001,N'Hà Nội - Vincom City Towers',N'Tầng 6, Toà nhà Vincom City Towers, 191 đường Bà Triệu, Quận Hai Bà Trưng, Hà Nội',N'+84 4 3 974 3333',21.01514,105.84906,N'',N'Hà Nội - Vincom City Towers')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1008,N'Hà Nội - Pico Mall',N'229 Tây Sơn, Quận Đống Đa, Hà Nội',N'04.6252 3333',21.00362,105.82092,N'',N'Hà Nội - Pico Mall')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1002,N'Hải Phòng - Thùy Dương Plaza',N'Tầng 5, TD Plaza, Ngã 5 sân bay Cát Bi, đường Lê Hồng Phong, Quận Ngô Quyền, Tp. Hải Phòng',N'031 365 3333',20.84678,106.70707,N'',N'Hải Phòng - Thùy Dương Plaza')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1005,N'Đà Nẵng - Vinh Trung Plaza',N'255-257 đường Hùng Vương, Quận Thanh Khê, Tp. Đà Nẵng',N'(0511) 3 666 222',16.06707,108.21207,N'',N'Đà Nẵng - Vinh Trung Plaza')
--insert into Cinemas(Id,DisplayName,Address,Phone,Latitude,Longitude,ImageUrl,Name) values(1003,N'Biên Hoà - Siêu thị Co-Op Mart',N'Tầng 3 | Siêu thị Co-Op Mart, 121 Quốc Lộ 15 | P. Tân Tiến, Tp. Biên Hoà, Tỉnh Đồng Nai',N'0613 940 222',10.95855,106.83400,N'',N'Biên Hoà - Siêu thị Co-Op Mart')
--SET IDENTITY_INSERT Cinemas OFF

--galaxy cinemas--
SET IDENTITY_INSERT Cinemas ON
insert into Cinemas(Id, CinemaWebId, Name, Address, Phone, Latitude, Longitude, ImageUrl, MapUrl) values(1,120,N'Galaxy Nguyễn Trãi',N'230 Nguyễn Trãi, Quận 1, Tp.HCM',N'(08) 3920 66 88',10.76497,106.68702,N'/images/upload/cinema/1328082093113-hinh chinh rap.png',N'/images/upload/cinema/1314073768481-6437.jpeg')
insert into Cinemas(Id, CinemaWebId, Name, Address, Phone, Latitude, Longitude, ImageUrl, MapUrl) values(2,122,N'Galaxy Nguyễn Du',N'116 Nguyễn Du, Quận 1, Tp.HCM',N'(08) 38 235 235',10.77324,106.69296,N'/images/upload/cinema/1328082229213-hinh chinh rap.png',N'/images/upload/cinema/1314152822341-4457.jpeg')
insert into Cinemas(Id, CinemaWebId, Name, Address, Phone, Latitude, Longitude, ImageUrl, MapUrl) values(3,124,N'Galaxy Tân Bình',N'246 Nguyễn Hồng Đào, Q.TB, Tp.HCM',N'(08) 3849 4567',10.79031,106.64077,N'/images/upload/cinema/1328082591679-170.png',N'/images/upload/cinema/1314152938481-8963.jpeg')
SET IDENTITY_INSERT Cinemas OFF

--provider & cinema--
insert into ProviderCinemas(Provider_Id, Cinema_Id) values (1,1) 
insert into ProviderCinemas(Provider_Id, Cinema_Id) values (1,2) 
insert into ProviderCinemas(Provider_Id, Cinema_Id) values (1,3) 
