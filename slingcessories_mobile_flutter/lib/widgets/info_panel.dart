import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

class InfoPanel extends StatelessWidget {
  final String? title;
  final Widget child;
  final IconData? icon;

  const InfoPanel({
    super.key,
    this.title,
    required this.child,
    this.icon,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.infoPanelBg,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(
          color: AppColors.infoPanelBorder,
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisSize: MainAxisSize.min,
        children: [
          if (title != null)
            Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: Row(
                children: [
                  if (icon != null) ...[
                    Icon(
                      icon,
                      color: AppColors.accentBlue,
                      size: 20,
                    ),
                    const SizedBox(width: 8),
                  ],
                  Text(
                    title!,
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.w600,
                      color: AppColors.accentBlue,
                    ),
                  ),
                ],
              ),
            ),
          child,
        ],
      ),
    );
  }
}

