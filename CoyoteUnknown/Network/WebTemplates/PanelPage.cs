namespace CoyoteUnknown.Network.WebTemplates
{
    public static class PanelPage
    {
        public static string GetHtml(string ip)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Coyote Unknown 控制面板</title>
    <style>
        {GetCss()}
    </style>
</head>
<body onload=""initPage()"">
    {GetBody(ip)}
    <script>
        {GetJavaScript()}
    </script>
</body>
</html>";
        }

        private static string GetCss()
        {
            return @"
                body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; background: #121212; color: #e0e0e0; max-width: 650px; margin: 20px auto; padding: 20px; text-align: center; transition: max-width 0.3s ease; }
                .card { background: #1e1e1e; padding: 28px; border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.6); border: 1px solid #333; text-align: left; }
                
                @media (min-width: 1050px) {
                    .card.pro-layout {
                        max-width: 1200px;
                        display: grid;
                        grid-template-columns: 1fr 1fr;
                        gap: 24px;
                        box-sizing: border-box;
                    }
                    body.pro-layout {
                        max-width: 1250px;
                    }
                }

                h1 { color: #ff9800; font-size: 24px; margin: 0 0 10px 0; text-align: center; grid-column: span 2; }
                .status { margin-bottom: 20px; text-align: center; font-size: 14px; color: #aaa; grid-column: span 2; }
                .section { background: #262626; padding: 18px; border-radius: 10px; margin-bottom: 18px; border: 1px solid #3c3c3c; height: fit-content; }
                h3 { margin: 0 0 14px 0; font-size: 16px; color: #ff9800; border-left: 4px solid #ff9800; padding-left: 8px; }

                .row { display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; }
                .row:last-child { margin-bottom: 0; }
                label { font-size: 14px; font-weight: bold; width: 220px; }
                select, input[type=range] { flex-grow: 1; margin: 0 16px; accent-color: #ff9800; }
                select { background: #333; color: #fff; border: 1px solid #555; padding: 6px; border-radius: 4px; outline: none; }
                .val { font-size: 14px; font-weight: bold; color: #ff9800; width: 55px; text-align: right; }

                .btn { background: #2196f3; color: white; border: none; padding: 8px 16px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; }
                .btn:hover { background: #42a5f5; }
                .btn-test { background: #e53935; }
                .btn-test:hover { background: #ef5350; }
                .switch-input { cursor: pointer; transform: scale(1.2); }

                .pro-only { display: none; }
                .pro-section { display: none; }
                .col-span-2 { grid-column: span 2; }

                .log-console { background: #000; border: 1px solid #444; border-radius: 8px; height: 140px; padding: 12px; font-family: 'Consolas', monospace; font-size: 12px; overflow-y: auto; color: #00ff00; box-sizing: border-box; }
                .log-line { margin-bottom: 4px; }
            ";
        }

        private static string GetBody(string ip)
        {
            return $@"
                <div class=""card"">
                    <h1 class=""col-span-2"">Coyote Unknown 控制面板</h1>
                    <div class=""status col-span-2"">
                        局域网探测 IP: <strong style=""color: #2196f3;"">{ip}</strong> | 绑定状态: <strong style=""color: green;"">已配对</strong>
                    </div>

                    <div class=""left-column"">
                        <div class=""section"">
                            <h3>联动模式主控设置</h3>
                            <div class=""row"">
                                <label>开启专业模式</label>
                                <input type=""checkbox"" id=""cfg-pro-mode"" class=""switch-input"" onchange=""setConfig('IsProfessionalMode', this.checked); toggleProMode(this.checked)"">
                            </div>
                            <div class=""row pro-only"">
                                <label>强度控制逻辑模式</label>
                                <select id=""cfg-control-mode"" onchange=""setConfig('StrengthControlMode', this.value)"">
                                    <option value=""FollowClient"">遵循客户端滑块</option>
                                    <option value=""FollowMod"">遵循 Mod 设定</option>
                                </select>
                            </div>
                            <div class=""row pro-only"">
                                <label>电击强度滑块显示模式</label>
                                <input type=""checkbox"" id=""cfg-abs-mode"" class=""switch-input"" onchange=""setConfig('UseAbsoluteValue', this.checked); toggleAbsMode(this.checked)"">
                            </div>
                        </div>

                        <div class=""section"">
                            <h3>物理遥控调试</h3>
                            <div class=""row"">
                                <label>通道 A 实时强度</label>
                                <input type=""range"" id=""slider-a"" min=""0"" max=""100"" value=""0"" onchange=""setStrength('A', this.value)"" oninput=""updateValText('val-a', this.value)"">
                                <span class=""val""><span id=""val-a"">0</span></span>
                            </div>
                            <div class=""row"">
                                <label>通道 B 实时强度</label>
                                <input type=""range"" id=""slider-b"" min=""0"" max=""100"" value=""0"" onchange=""setStrength('B', this.value)"" oninput=""updateValText('val-b', this.value)"">
                                <span class=""val""><span id=""val-b"">0</span></span>
                            </div>
                            <div class=""row"" style=""margin-top:14px; justify-content: flex-start; gap: 10px;"">
                                <button class=""btn btn-test"" onclick=""testChannel('A')"">测试放电 A</button>
                                <button class=""btn btn-test"" onclick=""testChannel('B')"">测试放电 B</button>
                            </div>
                        </div>

                        <div class=""section"">
                            <h3>基础联动配置</h3>
                            <div class=""row"">
                                <label>开启受击/自残瞬间反馈</label>
                                <input type=""checkbox"" id=""cfg-instant"" class=""switch-input"" onchange=""setConfig('EnableInstantShock', this.checked)"">
                            </div>
                            <div class=""row"">
                                <label>开启生理体征持续反馈</label>
                                <input type=""checkbox"" id=""cfg-continuous"" class=""switch-input"" onchange=""setConfig('EnableContinuousFeedback', this.checked)"">
                            </div>
                            <div class=""row pro-only"">
                                <label>全局放电电击倍率</label>
                                <input type=""range"" id=""cfg-global-mult"" min=""0.5"" max=""5.0"" step=""0.1"" onchange=""setConfig('GlobalMultiplier', this.value)"" oninput=""updateValText('val-cfg-global-mult', this.value)"">
                                <span class=""val"" id=""val-cfg-global-mult"" style=""color:#00ff00;"">1.0</span>
                            </div>
                            <div class=""row"">
                                <label>安全放电上限 A</label>
                                <input type=""range"" id=""cfg-max-a"" min=""0"" max=""100"" onchange=""setConfig('MaxStrengthA', this.value)"" oninput=""updateValText('val-cfg-max-a', this.value)"">
                                <span class=""val""><span id=""val-cfg-max-a"">20</span><span class=""unit-label"">%</span></span>
                            </div>
                            <div class=""row"">
                                <label>安全放电上限 B</label>
                                <input type=""range"" id=""cfg-max-b"" min=""0"" max=""100"" onchange=""setConfig('MaxStrengthB', this.value)"" oninput=""updateValText('val-cfg-max-b', this.value)"">
                                <span class=""val""><span id=""val-cfg-max-b"">15</span><span class=""unit-label"">%</span></span>
                            </div>
                        </div>

                        <div class=""section pro-section"">
                            <h3>Mod 侧电击绝对上限保障</h3>
                            <div class=""row"">
                                <label>启用 Mod 绝对上限强制限制</label>
                                <input type=""checkbox"" id=""cfg-enable-modclamp"" class=""switch-input"" onchange=""setConfig('EnableModClamp', this.checked)"">
                            </div>
                            <div class=""row"">
                                <label>通道 A 绝对上限 (强制限制)</label>
                                <input type=""range"" id=""cfg-modclamp-a"" min=""0"" max=""200"" onchange=""setConfig('ModClampA', this.value)"" oninput=""updateValText('val-cfg-modclamp-a', this.value)"">
                                <span class=""val"" id=""val-cfg-modclamp-a"">20</span>
                            </div>
                            <div class=""row"">
                                <label>通道 B 绝对上限 (强制限制)</label>
                                <input type=""range"" id=""cfg-modclamp-b"" min=""0"" max=""200"" onchange=""setConfig('ModClampB', this.value)"" oninput=""updateValText('val-cfg-modclamp-b', this.value)"">
                                <span class=""val"" id=""val-cfg-modclamp-b"">20</span>
                            </div>
                        </div>
                    </div>

                    <div class=""right-column pro-section"">
                        <!-- 高阶参数 -->
                        <div class=""section"">
                            <h3>持续基础联动微调</h3>
                            <div class=""row"">
                                <label>失血伤害电击折算倍率</label>
                                <input type=""range"" id=""cfg-dmg-mult"" min=""0.5"" max=""5.0"" step=""0.1"" onchange=""setConfig('DamageMultiplier', this.value)"" oninput=""updateValText('val-cfg-dmg', this.value)"">
                                <span class=""val"" id=""val-cfg-dmg"">1.5</span>
                            </div>
                            <div class=""row"">
                                <label>持续放电起搏最低心率</label>
                                <input type=""range"" id=""cfg-min-hr"" min=""60"" max=""150"" step=""5"" onchange=""setConfig('MinHeartRateToShock', this.value)"" oninput=""updateValText('val-cfg-hr', this.value)"">
                                <span class=""val"" id=""val-cfg-hr"">100</span>
                            </div>
                        </div>

                        <div class=""section"">
                            <h3>5 类核心细分生理 Debuff 联动</h3>
                            
                            <!-- 辐射病 -->
                            <div class=""row"" style=""border-bottom: 1px dashed #444; padding-bottom: 8px;"">
                                <label>辐射病联动反馈</label>
                                <input type=""checkbox"" id=""cfg-enable-rad"" class=""switch-input"" onchange=""setConfig('EnableRadiation', this.checked)"">
                            </div>
                            <div class=""row"">
                                <label>辐射放电权重倍率</label>
                                <input type=""range"" id=""cfg-rad-mult"" min=""0.1"" max=""3.0"" step=""0.1"" onchange=""setConfig('RadiationMultiplier', this.value)"" oninput=""updateValText('val-cfg-rad', this.value)"">
                                <span class=""val"" id=""val-cfg-rad"">1.0</span>
                            </div>
                            <div class=""row"" style=""margin-bottom: 16px;"">
                                <label>辐射病对应波形</label>
                                <select id=""cfg-wave-rad"" onchange=""setConfig('RadiationWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (重击波)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>

                            <div class=""row"" style=""border-bottom: 1px dashed #444; padding-bottom: 8px;"">
                                <label>失温症联动反馈</label>
                                <input type=""checkbox"" id=""cfg-enable-cold"" class=""switch-input"" onchange=""setConfig('EnableCold', this.checked)"">
                            </div>
                            <div class=""row"">
                                <label>失温放电权重倍率</label>
                                <input type=""range"" id=""cfg-cold-mult"" min=""0.1"" max=""3.0"" step=""0.1"" onchange=""setConfig('ColdMultiplier', this.value)"" oninput=""updateValText('val-cfg-cold', this.value)"">
                                <span class=""val"" id=""val-cfg-cold"">1.2</span>
                            </div>
                            <div class=""row"" style=""margin-bottom: 16px;"">
                                <label>失温症对应波形</label>
                                <select id=""cfg-wave-cold"" onchange=""setConfig('ColdWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (重击波)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>

                            <div class=""row"" style=""border-bottom: 1px dashed #444; padding-bottom: 8px;"">
                                <label>缺氧/窒息反馈</label>
                                <input type=""checkbox"" id=""cfg-enable-hypoxia"" class=""switch-input"" onchange=""setConfig('EnableHypoxia', this.checked)"">
                            </div>
                            <div class=""row"">
                                <label>缺氧放电权重倍率</label>
                                <input type=""range"" id=""cfg-hypoxia-mult"" min=""0.1"" max=""3.0"" step=""0.1"" onchange=""setConfig('HypoxiaMultiplier', this.value)"" oninput=""updateValText('val-cfg-hypoxia', this.value)"">
                                <span class=""val"" id=""val-cfg-hypoxia"">1.5</span>
                            </div>
                            <div class=""row"" style=""margin-bottom: 16px;"">
                                <label>缺氧对应波形</label>
                                <select id=""cfg-wave-hypoxia"" onchange=""setConfig('HypoxiaWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (重击波)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>

                            <div class=""row"" style=""border-bottom: 1px dashed #444; padding-bottom: 8px;"">
                                <label>肢体骨折/脱臼反馈</label>
                                <input type=""checkbox"" id=""cfg-enable-frac"" class=""switch-input"" onchange=""setConfig('EnableFracture', this.checked)"">
                            </div>
                            <div class=""row"">
                                <label>骨折放电权重倍率</label>
                                <input type=""range"" id=""cfg-frac-mult"" min=""0.1"" max=""3.0"" step=""0.1"" onchange=""setConfig('FractureMultiplier', this.value)"" oninput=""updateValText('val-cfg-frac', this.value)"">
                                <span class=""val"" id=""val-cfg-frac"">1.2</span>
                            </div>
                            <div class=""row"" style=""margin-bottom: 16px;"">
                                <label>骨折状态对应波形</label>
                                <select id=""cfg-wave-frac"" onchange=""setConfig('FractureWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (重击波)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>

                            <div class=""row"" style=""border-bottom: 1px dashed #444; padding-bottom: 8px;"">
                                <label>身体中毒/蛇毒反馈</label>
                                <input type=""checkbox"" id=""cfg-enable-poison"" class=""switch-input"" onchange=""setConfig('EnablePoison', this.checked)"">
                            </div>
                            <div class=""row"">
                                <label>中毒放电权重倍率</label>
                                <input type=""range"" id=""cfg-poison-mult"" min=""0.1"" max=""3.0"" step=""0.1"" onchange=""setConfig('PoisonMultiplier', this.value)"" oninput=""updateValText('val-cfg-poison', this.value)"">
                                <span class=""val"" id=""val-cfg-poison"">1.0</span>
                            </div>
                            <div class=""row"">
                                <label>中毒对应波形</label>
                                <select id=""cfg-wave-poison"" onchange=""setConfig('PoisonWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (重击波)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>
                        </div>

                        <div class=""section"">
                            <h3>基础联动波形细化绑定</h3>
                            <div class=""row"">
                                <label>瞬间失血波形绑定</label>
                                <select id=""cfg-wave-blood"" onchange=""setConfig('BloodLossWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (大出血重击)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>
                            <div class=""row"">
                                <label>瞬间剧痛波形绑定</label>
                                <select id=""cfg-wave-pain"" onchange=""setConfig('PainWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (大出血重击)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>
                            <div class=""row"">
                                <label>持续高心率波形绑定</label>
                                <select id=""cfg-wave-hr"" onchange=""setConfig('HeartRateWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (大出血重击)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>
                            <div class=""row"">
                                <label>持续肾上腺素波形</label>
                                <select id=""cfg-wave-adren"" onchange=""setConfig('AdrenalineWaveform', this.value)"">
                                    <option value=""HeavyShock"">HeavyShock (大出血重击)</option>
                                    <option value=""Sting"">Sting (尖锐刺痛波)</option>
                                    <option value=""Heartbeat"">Heartbeat (生理起搏)</option>
                                    <option value=""SoftBuzz"">SoftBuzz (酥麻背景波)</option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class=""section col-span-2"">
                        <div class=""log-console"" id=""console-box"">
                            <div class=""log-line"">[System] 正在读取系统日志流...</div>
                        </div>
                    </div>
                </div>
            ";
        }

        private static string GetJavaScript()
        {
            return @"
                function setStrength(ch, val) {
                    fetch('/api/setstrength?channel=' + ch + '&value=' + val);
                }
                function updateValText(id, val) {
                    document.getElementById(id).innerText = val;
                }
                function testChannel(ch) {
                    fetch('/api/test?channel=' + ch);
                }
                function setConfig(key, val) {
                    fetch('/api/setconfig?key=' + key + '&value=' + val);
                }

                function toggleProMode(isPro) {
                    const card = document.querySelector('.card');
                    const body = document.body;
                    if (isPro) {
                        card.classList.add('pro-layout');
                        body.classList.add('pro-layout');
                    } else {
                        card.classList.remove('pro-layout');
                        body.classList.remove('pro-layout');
                    }

                    const proOnlyRows = document.querySelectorAll('.pro-only');
                    proOnlyRows.forEach(el => el.style.display = isPro ? 'flex' : 'none');
                    const proSections = document.querySelectorAll('.pro-section');
                    proSections.forEach(el => el.style.display = isPro ? 'block' : 'none');
                }

                function toggleAbsMode(useAbs) {
                    const unitLabels = document.querySelectorAll('.unit-label');
                    unitLabels.forEach(el => el.innerText = useAbs ? '' : '%');
                    
                    const maxASlider = document.getElementById('cfg-max-a');
                    const maxBSlider = document.getElementById('cfg-max-b');
                    if (useAbs) {
                        maxASlider.max = '200';
                        maxBSlider.max = '200';
                    } else {
                        maxASlider.max = '100';
                        maxBSlider.max = '100';
                        if (parseInt(maxASlider.value) > 100) maxASlider.value = 100;
                        if (parseInt(maxBSlider.value) > 100) maxBSlider.value = 100;
                        document.getElementById('val-cfg-max-a').innerText = maxASlider.value;
                        document.getElementById('val-cfg-max-b').innerText = maxBSlider.value;
                    }
                }

                function initPage() {
                    fetch('/api/getconfig')
                        .then(r => r.json())
                        .then(cfg => {
                            document.getElementById('cfg-instant').checked = cfg.enableInstant;
                            document.getElementById('cfg-continuous').checked = cfg.enableContinuous;

                            document.getElementById('cfg-dmg-mult').value = cfg.damageMultiplier;
                            document.getElementById('val-cfg-dmg').innerText = cfg.damageMultiplier;

                            document.getElementById('cfg-min-hr').value = cfg.minHeartRate;
                            document.getElementById('val-cfg-hr').innerText = cfg.minHeartRate;

                            document.getElementById('cfg-max-a').value = cfg.maxStrengthA;
                            document.getElementById('val-cfg-max-a').innerText = cfg.maxStrengthA;

                            document.getElementById('cfg-max-b').value = cfg.maxStrengthB;
                            document.getElementById('val-cfg-max-b').innerText = cfg.maxStrengthB;

                            document.getElementById('cfg-pro-mode').checked = cfg.isProfessionalMode;
                            document.getElementById('cfg-control-mode').value = cfg.strengthControlMode;
                            document.getElementById('cfg-abs-mode').checked = cfg.useAbsoluteValue;
                            document.getElementById('cfg-global-mult').value = cfg.globalMultiplier;
                            document.getElementById('val-cfg-global-mult').innerText = cfg.globalMultiplier;

                            document.getElementById('cfg-enable-modclamp').checked = cfg.enableModClamp;
                            document.getElementById('cfg-modclamp-a').value = cfg.modClampA;
                            document.getElementById('val-cfg-modclamp-a').innerText = cfg.modClampA;
                            document.getElementById('cfg-modclamp-b').value = cfg.modClampB;
                            document.getElementById('val-cfg-modclamp-b').innerText = cfg.modClampB;

                            document.getElementById('cfg-enable-rad').checked = cfg.enableRad;
                            document.getElementById('cfg-rad-mult').value = cfg.radMult;
                            document.getElementById('val-cfg-rad').innerText = cfg.radMult;
                            document.getElementById('cfg-wave-rad').value = cfg.radWave;

                            document.getElementById('cfg-enable-cold').checked = cfg.enableCold;
                            document.getElementById('cfg-cold-mult').value = cfg.coldMult;
                            document.getElementById('val-cfg-cold').innerText = cfg.coldMult;
                            document.getElementById('cfg-wave-cold').value = cfg.coldWave;

                            document.getElementById('cfg-enable-hypoxia').checked = cfg.enableHypoxia;
                            document.getElementById('cfg-hypoxia-mult').value = cfg.hypoxiaMult;
                            document.getElementById('val-cfg-hypoxia').innerText = cfg.hypoxiaMult;
                            document.getElementById('cfg-wave-hypoxia').value = cfg.hypoxiaWave;

                            document.getElementById('cfg-enable-frac').checked = cfg.enableFrac;
                            document.getElementById('cfg-frac-mult').value = cfg.fracMult;
                            document.getElementById('val-cfg-frac').innerText = cfg.fracMult;
                            document.getElementById('cfg-wave-frac').value = cfg.fracWave;

                            document.getElementById('cfg-enable-poison').checked = cfg.enablePoison;
                            document.getElementById('cfg-poison-mult').value = cfg.poisonMult;
                            document.getElementById('val-cfg-poison').innerText = cfg.poisonMult;
                            document.getElementById('cfg-wave-poison').value = cfg.poisonWave;

                            document.getElementById('cfg-wave-blood').value = cfg.bloodLossWaveform;
                            document.getElementById('cfg-wave-pain').value = cfg.painWaveform;
                            document.getElementById('cfg-wave-hr').value = cfg.heartRateWaveform;
                            document.getElementById('cfg-wave-adren').value = cfg.adrenalineWaveform;

                            toggleProMode(cfg.isProfessionalMode);
                            toggleAbsMode(cfg.useAbsoluteValue);
                        });
                }

                setInterval(() => {
                    fetch('/api/status')
                        .then(r => r.json())
                        .then(data => {
                            if (!data.isBound) {
                                window.location.href = '/';
                                return;
                            }
                            const consoleBox = document.getElementById('console-box');
                            consoleBox.innerHTML = '';
                            data.logs.forEach(logLine => {
                                const div = document.createElement('div');
                                div.className = 'log-line';
                                div.innerText = logLine;
                                consoleBox.appendChild(div);
                            });
                            consoleBox.scrollTop = consoleBox.scrollHeight;
                        });
                }, 1000);
            ";
        }
    }
}