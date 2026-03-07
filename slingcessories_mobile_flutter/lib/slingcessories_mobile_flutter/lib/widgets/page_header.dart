import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

class PageHeader extends StatelessWidget {
  final String title;
  final IconData? icon;
  final List<Widget>? actions;

  const PageHeader({
    super.key,
    required this.title,
    this.icon,
    this.actions,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.backgroundWhite,
        border: Border(
          left: const BorderSide(
            color: AppColors.accentAmber,
            width: 4,
          ),
          bottom: BorderSide(
            color: Colors.grey.shade200,
            width: 1,
          ),
        ),
      ),
      child: Row(
        children: [
          if (icon != null) ...[
            Icon(icon, color: AppColors.accentAmber, size: 28),
            const SizedBox(width: 12),
          ],
          Expanded(
            child: Text(
              title,
              style: const TextStyle(
                fontSize: 28,
                fontWeight: FontWeight.bold,
                color: AppColors.textDark,
              ),
            ),
          ),
          if (actions != null) ...actions!,
        ],
      ),
    );
  }
}

