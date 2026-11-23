# Release手順

## 手順

1. 既定ブランチへリリース対象を取り込みます。
2. `docs/CHANGELOG.md`のリリース内容を確認します。
3. バージョン形式のタグを作成します。

   ```powershell
   git tag v2.0.0
   ```

4. タグをpushします。

   ```powershell
   git push origin v2.0.0
   ```

5. GitHub Actionsの`Release`ワークフロー完了後、GitHub ReleaseのEXEを確認します。

## 技術詳細

`Release`ワークフローは`v`で始まるタグに反応します。Windows runner上で.NET 8向けに`win-x64`、self-contained、single-fileで発行し、`.exe`をGitHub Releaseへ添付します。

配布物はQPDFのネイティブライブラリを含めた1つのEXEです。実行時に.NETランタイムの別途インストールは不要です。
