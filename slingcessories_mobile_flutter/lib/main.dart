import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'providers/accessories_provider.dart';
import 'providers/categories_provider.dart';
import 'providers/slingshots_provider.dart';
import 'providers/users_provider.dart';
import 'pages/home_page.dart';
import 'pages/accessories_page.dart';
import 'pages/categories_page.dart';
import 'pages/slingshots_page.dart';
import 'pages/users_page.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AccessoriesProvider()),
        ChangeNotifierProvider(create: (_) => CategoriesProvider()),
        ChangeNotifierProvider(create: (_) => SlingshotsProvider()),
        ChangeNotifierProvider(create: (_) => UsersProvider()),
      ],
      child: MaterialApp(
        title: 'Slingcessories',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
          useMaterial3: true,
        ),
        initialRoute: '/',
        routes: {
          '/': (context) => const HomePage(),
          '/accessories': (context) => const AccessoriesPage(),
          '/categories': (context) => const CategoriesPage(),
          '/slingshots': (context) => const SlingshotsPage(),
          '/users': (context) => const UsersPage(),
        },
      ),
    );
  }
}

