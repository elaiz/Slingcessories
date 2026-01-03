import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../theme/app_theme.dart';
import '../widgets/page_header.dart';
import '../widgets/info_panel.dart';
import '../providers/users_provider.dart';

class HomePage extends StatefulWidget {
  final void Function(int)? onNavigateToTab;
  
  const HomePage({super.key, this.onNavigateToTab});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  String? _selectedUserId;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<UsersProvider>().loadUsers();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.backgroundLight,
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const PageHeader(title: 'Slingcessories!'),
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    InfoPanel(
                      title: 'Welcome Back!',
                      icon: Icons.info_outline,
                      child: Consumer<UsersProvider>(
                        builder: (context, provider, child) {
                          return Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
                                'Please select your user account to continue:',
                                style: TextStyle(
                                  color: AppColors.accentBlue,
                                  fontSize: 14,
                                ),
                              ),
                              const SizedBox(height: 12),
                              Container(
                                padding: const EdgeInsets.symmetric(horizontal: 12),
                                decoration: BoxDecoration(
                                  color: AppColors.backgroundWhite,
                                  borderRadius: BorderRadius.circular(6),
                                  border: Border.all(color: AppColors.textLight),
                                ),
                                child: DropdownButtonHideUnderline(
                                  child: DropdownButton<String>(
                                    isExpanded: true,
                                    hint: Text(
                                      'Select User',
                                      style: TextStyle(
                                        color: AppColors.accentBlue.withOpacity(0.7),
                                      ),
                                    ),
                                    value: _selectedUserId,
                                    items: [
                                      DropdownMenuItem<String>(
                                        value: null,
                                        child: Text(
                                          'Select User',
                                          style: TextStyle(
                                            color: AppColors.accentBlue.withOpacity(0.7),
                                          ),
                                        ),
                                      ),
                                      if (!provider.isLoading)
                                        ...provider.users.map((user) {
                                          return DropdownMenuItem<String>(
                                            value: user.id,
                                            child: Text(
                                              user.fullName,
                                              style: const TextStyle(
                                                color: AppColors.textDark,
                                              ),
                                            ),
                                          );
                                        }),
                                    ],
                                    onChanged: (value) {
                                      setState(() {
                                        _selectedUserId = value;
                                      });
                                    },
                                  ),
                                ),
            ),
                              const SizedBox(height: 12),
                              Row(
                                children: [
                                  const Text(
                                    'Or ',
                                    style: TextStyle(
                                      color: AppColors.accentBlue,
                                      fontSize: 14,
                                    ),
                                  ),
                                  TextButton(
              onPressed: () {
                                      _showRegisterDialog(context);
                                    },
                                    style: TextButton.styleFrom(
                                      padding: EdgeInsets.zero,
                                      minimumSize: Size.zero,
                                      tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                                    ),
                                    child: const Text(
                                      'register a new account',
                                      style: TextStyle(
                                        color: AppColors.accentBlue,
                                        fontSize: 14,
                                        decoration: TextDecoration.underline,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            ],
                          );
                        },
                      ),
            ),
                    const SizedBox(height: 24),
                    // Quick actions grid
                    const Text(
                      'Quick Actions',
                      style: TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                        color: AppColors.textDark,
                      ),
                    ),
                    const SizedBox(height: 12),
                    GridView.count(
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      crossAxisCount: 2,
                      mainAxisSpacing: 12,
                      crossAxisSpacing: 12,
                      childAspectRatio: 1.5,
                      children: [
                        _buildQuickActionCard(
                          context,
                          icon: Icons.category,
                          title: 'Slingshots',
                          color: AppColors.primaryDark,
                          onTap: () => _navigateToTab(context, 1),
                        ),
                        _buildQuickActionCard(
                          context,
                          icon: Icons.list_alt,
                          title: 'Accessories',
                          color: AppColors.accentGreen,
                          onTap: () => _navigateToTab(context, 2),
                        ),
                        _buildQuickActionCard(
                          context,
                          icon: Icons.star,
                          title: 'Wishlist',
                          color: AppColors.accentAmber,
                          onTap: () => _navigateToTab(context, 2),
                        ),
                        _buildQuickActionCard(
                          context,
                          icon: Icons.person,
                          title: 'Users',
                          color: AppColors.accentBlue,
                          onTap: () => _navigateToTab(context, 3),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildQuickActionCard(
    BuildContext context, {
    required IconData icon,
    required String title,
    required Color color,
    required VoidCallback onTap,
  }) {
    return Card(
      elevation: 2,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(8),
        child: Container(
          padding: const EdgeInsets.all(16),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(icon, size: 32, color: color),
              const SizedBox(height: 8),
              Text(
                title,
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                  color: color,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _navigateToTab(BuildContext context, int index) {
    if (widget.onNavigateToTab != null) {
      widget.onNavigateToTab!(index);
    }
  }

  void _showRegisterDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Register'),
        content: const Text('Registration feature coming soon!'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }
}
